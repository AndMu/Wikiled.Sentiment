using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Logging;
using Wikiled.Console.Arguments;
using Wikiled.Sentiment.Analysis.Containers;
using Wikiled.Sentiment.Analysis.Pipeline;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.ConsoleApp.Extraction
{
    [Description("Extract Aspects and features from unlaballed dataset")]
    public class ExtractAttributesCommand : Command
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<ExtractAttributesCommand>();

        private MainAspectHandler featureExtractor;

        private ISessionContainer container;

        public override string Name { get; } = "extract";

        [Description("Source of documents")]
        [Required]
        public string Source { get; set; }

        [Description("Place to save output")]
        [Required]
        public string Out { get; set; }

        [Description("Include sentiment words into attributes")]
        public bool Sentiment { get; set; }

        protected override async Task Execute(CancellationToken token)
        {
            log.LogInformation("Starting...");
            featureExtractor = new MainAspectHandler(new AspectContextFactory(Sentiment));
            container = MainContainerFactory.CreateStandard().Create().StartSession();
            IProcessingPipeline pipeline = container.Resolve<IProcessingPipeline>();
            using (Observable.Interval(TimeSpan.FromSeconds(30)).Subscribe(item => log.LogInformation(pipeline.Monitor.ToString())))
            {
                await pipeline.ProcessStep(GetReviews().ToObservable(TaskPoolScheduler.Default))
                        .Select(item => Observable.Start(() =>
                        {
                            Processing(item);
                            pipeline.Monitor.Increment();
                        }))
                        .Merge()
                        .LastOrDefaultAsync();
            }

            string file = Path.Combine(Out, "features.xml");
            log.LogInformation("Saving {0}...", file);
            IAspectSerializer serializer = container.Resolve<IAspectSerializer>();
            serializer.Serialize(featureExtractor).Save(file);
        }

        private IEnumerable<IParsedDocumentHolder> GetReviews()
        {
            log.LogInformation("Input {0}", Source);
            return container.GetTextSplitter().GetParsedReviewHolders(Source, null);
        }

        private void Processing(ProcessingContext reviewHolder)
        {
            featureExtractor.Process(reviewHolder.Review);
        }
    }
}
