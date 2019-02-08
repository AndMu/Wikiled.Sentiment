using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Console.Arguments;
using Wikiled.Sentiment.Analysis.Containers;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Persistency;
using Wikiled.Sentiment.ConsoleApp.Analysis.Config;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.ConsoleApp.Analysis
{
    public abstract class BaseRawCommand<T> : Command
        where T : BaseRawConfig
    {
        private readonly ISessionContainer container;

        private readonly IDataLoader loader;

        protected BaseRawCommand(ILogger log, T config, IDataLoader loader, ISessionContainer container)
            : base(log)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            this.container = container ?? throw new ArgumentNullException(nameof(container));
            this.loader = loader;
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

            IObservable<IParsedDocumentHolder> review = GetAllDocuments();
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

        private IObservable<IParsedDocumentHolder> GetAllDocuments()
        {
            Logger.LogInformation("Loading {0}", Config);
            var dataSource = loader.Load(Config);
            return container.GetTextSplitter().GetParsedReviewHolders(dataSource);
        }
    }
}
