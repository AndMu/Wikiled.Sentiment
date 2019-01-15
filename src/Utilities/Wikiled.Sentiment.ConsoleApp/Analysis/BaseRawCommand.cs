using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Console.Arguments;
using Wikiled.Sentiment.Analysis.Containers;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.ConsoleApp.Analysis.Config;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.ConsoleApp.Analysis
{
    public abstract class BaseRawCommand<T> : Command
        where T : BaseRawConfig
    {
        private readonly ILogger log;

        private ISessionContainer container;

        protected BaseRawCommand(ILogger log, T config, ISessionContainer container)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            this.container = container ?? throw new ArgumentNullException(nameof(container));
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        protected SemaphoreSlim Semaphore { get; set; }

        public T Config { get; }

        protected override Task Execute(CancellationToken token)
        {
            log.LogInformation("Initialize...");
            container.Context.DisableFeatureSentiment = Config.InvertOff;
            log.LogInformation("Processing...");
            ISentimentDataHolder sentimentAdjustment = default;
            if (!string.IsNullOrEmpty(Config.Weights))
            {
                log.LogInformation("Adjusting Embeddings sentiments using [{0}] ...", Config.Weights);
                sentimentAdjustment = SentimentDataHolder.Load(Config.Weights);
            }

            IObservable<IParsedDocumentHolder> review;

            if (!string.IsNullOrEmpty(Config.Input))
            {
                review = GetOtherReviews();
            }
            else if (string.IsNullOrEmpty(Config.Positive))
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
            log.LogInformation("Loading {0}", Config.Articles);
            IProcessingData data = new DataLoader().Load(Config.Articles);
            return container.GetTextSplitter().GetParsedReviewHolders(data);
        }

        private IObservable<IParsedDocumentHolder> GetPositiveReviews()
        {
            log.LogInformation("Positive {0}", Config.Positive);
            return container.GetTextSplitter().GetParsedReviewHolders(Config.Positive, true).ToObservable();
        }

        private IObservable<IParsedDocumentHolder> GetNegativeReviews()
        {
            log.LogInformation("Negative {0}", Config.Negative);
            return container.GetTextSplitter().GetParsedReviewHolders(Config.Negative, false).ToObservable();
        }

        private IObservable<IParsedDocumentHolder> GetOtherReviews()
        {
            log.LogInformation("Other {0}", Config.Input);
            return container.GetTextSplitter().GetParsedReviewHolders(Config.Input, null).ToObservable();
        }
    }
}
