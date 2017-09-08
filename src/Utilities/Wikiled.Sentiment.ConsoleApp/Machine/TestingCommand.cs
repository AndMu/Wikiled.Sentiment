using System;
using System.Reactive.Linq;
using NLog;
using Wikiled.Sentiment.Analysis.CrossDomain;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.ConsoleApp.Machine
{
    /// <summary>
    /// testing [-Year=2000 -Years=2] [-Raw=true] [-Word2Vec=C:\data.csv] [-Server=docs] -Port=6380 [-SvmPath=.\SvmTwo] -Category=Electronics
    /// </summary>
    public class TestingCommand : BaseRedis
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public bool Raw { get; set; }

        public string Word2Vec { get; set; }

        public override bool IsTraining => false;

        protected override void Processing(IObservable<IParsedDocumentHolder> reviews)
        {
            TestingClient client = new TestingClient(Helper, reviews, ProcessingPath);
            var name = Category.ToString();
            if (Year != null)
            {
                name += $"_{Year}";
            }

            if (Years != null)
            {
                name += $"_{Years}";
            }

            if (Raw)
            {
                log.Info("Using Raw testing");
                client.DisableAspects = true;
                client.DisableSvm = true;
                name += "_Raw";
            }

            if (!string.IsNullOrEmpty(Word2Vec))
            {
                log.Info("Adjusting Word2Vec sentiments...");
                name += "_Word2Vec";
                var adjuster = new WeightSentimentAdjuster(Helper.DataLoader.SentimentDataHolder);
                adjuster.Adjust(Word2Vec);
            }

            client.Init();
            client.Process().LastOrDefaultAsync().Wait();
            client.Holder.Save(name + ".csv");
            log.Info("Completed. Performance: {0}", client.GetPerformanceDescription());
        }
    }
}
