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
        private readonly ISessionContainer container;

        protected BaseRawCommand(ILogger log, T config, ISessionContainer container)
            : base(log)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            this.container = container ?? throw new ArgumentNullException(nameof(container));
        }

        protected SemaphoreSlim Semaphore { get; set; }

        public T Config { get; }

        protected override Task Execute(CancellationToken token)
        {
            Logger.LogInformation("Initialize...");
            container.Context.DisableFeatureSentiment = Config.InvertOff;
            Logger.LogInformation("Processing...");
            ISentimentDataHolder sentimentAdjustment = default;
            if (!string.IsNullOrEmpty(Config.Weights))
            {
                Logger.LogInformation("Adjusting Embeddings sentiments using [{0}] ...", Config.Weights);
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

            return Process(review.Select(SynchronizedReviews).Merge(), container, sentimentAdjustment);
        }

        protected abstract Task Process(IObservable<IParsedDocumentHolder> reviews, ISessionContainer container, ISentimentDataHolder sentimentAdjustment);

        private async Task<IParsedDocumentHolder> SynchronizedReviews(IParsedDocumentHolder review)
        {
            if (Semaphore != null)
            {
                await Semaphore.WaitAsync().ConfigureAwait(false);
            }

            return review;
        }

        private IObservable<IParsedDocumentHolder> GetAllReviews()
        {
            Logger.LogInformation("Loading {0}", Config.Articles);
            IProcessingData data = new DataLoader().Load(Config.Articles);
            return container.GetTextSplitter().GetParsedReviewHolders(data);
        }

        private IObservable<IParsedDocumentHolder> GetPositiveReviews()
        {
            Logger.LogInformation("Positive {0}", Config.Positive);
            return container.GetTextSplitter().GetParsedReviewHolders(Config.Positive, true).ToObservable();
        }

        private IObservable<IParsedDocumentHolder> GetNegativeReviews()
        {
            Logger.LogInformation("Negative {0}", Config.Negative);
            return container.GetTextSplitter().GetParsedReviewHolders(Config.Negative, false).ToObservable();
        }

        private IObservable<IParsedDocumentHolder> GetOtherReviews()
        {
            Logger.LogInformation("Other {0}", Config.Input);
            return container.GetTextSplitter().GetParsedReviewHolders(Config.Input, null).ToObservable();
        }
    }
}
