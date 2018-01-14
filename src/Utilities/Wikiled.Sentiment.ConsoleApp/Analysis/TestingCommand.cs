using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using NLog;
using Wikiled.Core.Utility.Logging;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Pipeline;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.ConsoleApp.Analysis
{
    /// <summary>
    /// test [-Articles="C:\Cloud\OneDrive\Study\Medical\articles.xml"] [-Trained=.\Svm] -Out=.\results
    /// </summary>
    [Description("pSenti testing")]
    public class TestingCommand : BaseRawCommand
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public override string Name { get; } = "test";

        /// <summary>
        /// Path to pretrained data. If empty will use as basic lexicon
        /// </summary>
        public string Trained { get; set; }

        [Required]
        public string Out { get; set; }

        protected override void Process(IEnumerable<IParsedDocumentHolder> reviews, ISplitterHelper splitter)
        {
            TestingClient client;
            var pipeline = new ProcessingPipeline(TaskPoolScheduler.Default, splitter, reviews.ToObservable(TaskPoolScheduler.Default));
            using (Observable.Interval(TimeSpan.FromSeconds(30))
                             .Subscribe(item => log.Info(pipeline.Monitor)))
            {
                client = new TestingClient(pipeline, Trained);
                client.UseBagOfWords = UseBagOfWords;
                client.Init();
                client.Process().LastOrDefaultAsync().Wait();
            }
            
            client.Save(Out);
            log.Info($"Testing performance {client.GetPerformanceDescription()}");
        }
    }
}
