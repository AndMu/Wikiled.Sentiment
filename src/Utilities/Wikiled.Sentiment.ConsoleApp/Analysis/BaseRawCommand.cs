using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Wikiled.Console.Arguments;
using Wikiled.Redis.Config;
using Wikiled.Redis.Logic;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Splitters;
using Wikiled.Sentiment.Text.Cache;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.ConsoleApp.Analysis
{
    public abstract class BaseRawCommand : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private IContainerHelper container;

        protected SemaphoreSlim Semaphore { get; set; }

        public string Weights { get; set; }

        public bool FullWeightReset { get; set; }

        public bool Redis { get; set; }

        public int? Port { get; set; }

        public string Articles { get; set; }

        public string Positive { get; set; }

        public string Negative { get; set; }

        public string Input { get; set; }

        public bool UseBagOfWords { get; set; }

        public bool InvertOff { get; set; }

        public POSTaggerType Tagger { get; set; } = POSTaggerType.SharpNLP;

        protected override Task Execute(CancellationToken token)
        {
            log.Info("Initialize...");
            ICacheFactory cacheFactory = new NullCacheFactory();
            if (Redis)
            {
                log.Info("Using REDIS...");
                int port = Port ?? 6370;
                var redis = new RedisLink("Twitter", new RedisMultiplexer(new RedisConfiguration("localhost", port)));
                redis.Open();
                cacheFactory = new RedisDocumentCacheFactory(redis);
            }

            container = new MainSplitterFactory(cacheFactory, new ConfigurationHandler()).Create(Tagger);
            throw new NotImplementedException();
            //container.DataLoader.DisableFeatureSentiment = InvertOff;
            //log.Info("Processing...");

            //if (!string.IsNullOrEmpty(Weights))
            //{
            //    log.Info("Adjusting Embeddings sentiments using [{0}] ...", Weights);
            //    if (FullWeightReset)
            //    {
            //        log.Info("Full weight reset");
            //        container.DataLoader.SentimentDataHolder.Clear();
            //    }

            //    var adjuster = new WeightSentimentAdjuster(container.DataLoader.SentimentDataHolder);
            //    adjuster.Adjust(Weights);
            //}

            IObservable<IParsedDocumentHolder> review;

            if (!string.IsNullOrEmpty(Input))
            {
                review = GetOtherReviews();
            }
            else if (string.IsNullOrEmpty(Positive))
            {
                review = GetAllReviews();
            }
            else
            {
                review = GetNegativeReviews().Concat(GetPositiveReviews());
            }
                             
            Process(review.Select(SynchronizedReviews), container);
            return Task.CompletedTask;
        }

        protected abstract void Process(IObservable<IParsedDocumentHolder> reviews, IContainerHelper container);

        private IParsedDocumentHolder SynchronizedReviews(IParsedDocumentHolder review)
        {
            Semaphore?.Wait();
            return review;
        }

        private IObservable<IParsedDocumentHolder> GetAllReviews()
        {
            log.Info("Loading {0}", Articles);
            var data = new DataLoader().Load(Articles);
            return container.GetTextSplitter().GetParsedReviewHolders(data);
        }

        private IObservable<IParsedDocumentHolder> GetPositiveReviews()
        {
            log.Info("Positive {0}", Positive);
            return container.GetTextSplitter().GetParsedReviewHolders(Positive, true).ToObservable();
        }

        private IObservable<IParsedDocumentHolder> GetNegativeReviews()
        {
            log.Info("Negative {0}", Negative);
            return container.GetTextSplitter().GetParsedReviewHolders(Negative, false).ToObservable();
        }

        private IObservable<IParsedDocumentHolder> GetOtherReviews()
        {
            log.Info("Other {0}", Input);
            return container.Splitter.GetParsedReviewHolders(Input, null).ToObservable();
        }
    }
}
