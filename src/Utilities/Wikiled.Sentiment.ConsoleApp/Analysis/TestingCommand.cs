using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

        protected override void Process(IObservable<IParsedDocumentHolder> reviews, ISplitterHelper splitter)
        {
            var monitor = new PerformanceMonitor(0);
            TestingClient client;
            using (Observable.Interval(TimeSpan.FromSeconds(30))
                             .Subscribe(item => log.Info(monitor)))
            {
                reviews = reviews.Select(
                    item =>
                        {
                            monitor.ManualyCount();
                            return item;
                        });

                client = new TestingClient(new ProcessingPipeline(splitter, reviews), Trained);
                client.UseBagOfWords = UseBagOfWords;
                client.Process().Select(
                    item =>
                        {
                            monitor.Increment();
                            return item;
                        }).LastOrDefaultAsync().Wait();
            }
            
            client.Save(Out);
            log.Info($"Testing performance {client.GetPerformanceDescription()}");
        }
    }
}
