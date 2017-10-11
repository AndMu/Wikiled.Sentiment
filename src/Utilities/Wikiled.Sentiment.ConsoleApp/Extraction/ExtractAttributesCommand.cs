using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Resources;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Pipeline;
using Wikiled.Sentiment.Analysis.Processing.Splitters;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.ConsoleApp.Extraction
{
    [Description("Extract Aspects and features from unlaballed dataset")]
    public class ExtractAttributesCommand : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private MainAspectHandler featureExtractor;

        private ISplitterHelper splitter;

        public override string Name { get; } = "extract";

        public string Input { get; set; }

        public override void Execute()
        {
            log.Info("Starting...");
            featureExtractor = new MainAspectHandler(new AspectContextFactory());
            splitter = new MainSplitterFactory(new LocalCacheFactory(), new ConfigurationHandler()).Create(POSTaggerType.SharpNLP);
            var pipeline = new ProcessingPipeline(
                TaskPoolScheduler.Default,
                splitter,
                GetReviews()
                    .ToObservable(TaskPoolScheduler.Default));
            using (Observable.Interval(TimeSpan.FromSeconds(30))
                             .Subscribe(item => log.Info(pipeline.Monitor)))
            {
                pipeline.ProcessStep()
                        .Select(item => Observable.Start(() => Processing(item)))
                        .Merge()
                        .LastOrDefaultAsync()
                        .Wait();
            }
        }

        private IEnumerable<IParsedDocumentHolder> GetReviews()
        {
            log.Info("Input {0}", Input);
            return splitter.Splitter.GetParsedReviewHolders(Input, null);
        }

        private void Processing(ProcessingContext reviewHolder)
        {
            featureExtractor.Process(reviewHolder.Review);
        }
    }
}
