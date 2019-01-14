using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Logging;
using Wikiled.Console.Arguments;
using Wikiled.Sentiment.Analysis.Containers;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.ConsoleApp.Analysis
{
    public abstract class BaseRawCommand : Command
    {
         private static readonly ILogger log = ApplicationLogging.CreateLogger<BaseRawCommand>();

        private ISessionContainer container;

        protected SemaphoreSlim Semaphore { get; set; }

        public string Weights { get; set; }

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
            log.LogInformation("Initialize...");

            var factory = MainContainerFactory.Setup()
                .Config()
                .SetupLocalCache()
                .Splitter();

            container = factory.Create().StartSession();
            container.Context.DisableFeatureSentiment = InvertOff;
            log.LogInformation("Processing...");
            ISentimentDataHolder sentimentAdjustment = default;
            if (!string.IsNullOrEmpty(Weights))
            {
                log.LogInformation("Adjusting Embeddings sentiments using [{0}] ...", Weights);
                sentimentAdjustment = SentimentDataHolder.Load(Weights);
            }

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
                             
            return Process(review.Select(SynchronizedReviews), container, sentimentAdjustment);
        }

        protected abstract Task Process(IObservable<IParsedDocumentHolder> reviews, ISessionContainer container, ISentimentDataHolder sentimentAdjustment);

        private IParsedDocumentHolder SynchronizedReviews(IParsedDocumentHolder review)
        {
            Semaphore?.Wait();
            return review;
        }

        private IObservable<IParsedDocumentHolder> GetAllReviews()
        {
            log.LogInformation("Loading {0}", Articles);
            var data = new DataLoader().Load(Articles);
            return container.GetTextSplitter().GetParsedReviewHolders(data);
        }

        private IObservable<IParsedDocumentHolder> GetPositiveReviews()
        {
            log.LogInformation("Positive {0}", Positive);
            return container.GetTextSplitter().GetParsedReviewHolders(Positive, true).ToObservable();
        }

        private IObservable<IParsedDocumentHolder> GetNegativeReviews()
        {
            log.LogInformation("Negative {0}", Negative);
            return container.GetTextSplitter().GetParsedReviewHolders(Negative, false).ToObservable();
        }

        private IObservable<IParsedDocumentHolder> GetOtherReviews()
        {
            log.LogInformation("Other {0}", Input);
            return container.GetTextSplitter().GetParsedReviewHolders(Input, null).ToObservable();
        }
    }
}
