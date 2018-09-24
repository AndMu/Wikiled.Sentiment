using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using NLog;
using Wikiled.Console.Arguments;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Containers;
using Wikiled.Sentiment.Analysis.Processing.Pipeline;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.ConsoleApp.Extraction
{
    [Description("Extract Aspects and features from unlaballed dataset")]
    public class ExtractAttributesCommand : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private MainAspectHandler featureExtractor;

        private IContainerHelper container;

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
            log.Info("Starting...");
            featureExtractor = new MainAspectHandler(new AspectContextFactory(Sentiment));
            container = MainContainerFactory.CreateStandard().Create();
            var pipeline = new ProcessingPipeline(TaskPoolScheduler.Default, container);
            using (Observable.Interval(TimeSpan.FromSeconds(30))
                             .Subscribe(item => log.Info(pipeline.Monitor)))
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

            var file = Path.Combine(Out, "features.xml");
            log.Info("Saving {0}...", file);
            var serializer = container.Container.Resolve<IAspectSerializer>();
            serializer.Serialize(featureExtractor).Save(file);
        }

        private IEnumerable<IParsedDocumentHolder> GetReviews()
        {
            log.Info("Input {0}", Source);
            return container.GetTextSplitter().GetParsedReviewHolders(Source, null);
        }

        private void Processing(ProcessingContext reviewHolder)
        {
            featureExtractor.Process(reviewHolder.Review);
        }
    }
}
