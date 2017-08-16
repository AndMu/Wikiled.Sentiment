using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Polly.Retry;
using Wikiled.Redis.Logic;
using Wikiled.Sentiment.AcceptanceTests.Helpers.Data;
using Wikiled.Sentiment.Analysis.Amazon;
using Wikiled.Sentiment.Analysis.Amazon.Logic;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.AcceptanceTests.Helpers
{
    public class TestRunner
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly SentimentTestData definition;

        private readonly TestHelper helper;

        private readonly SemaphoreSlim semaphore;

        private readonly RetryPolicy retryPolicy = RetryHandler.Construct(true);

        public TestRunner(TestHelper helper, SentimentTestData definition)
        {
            this.helper = helper;
            this.definition = definition;
            Active = definition.Cached ? helper.CachedSplitterHelper : helper.NonCachedSplitterHelper;
            var maxParallel = Environment.ProcessorCount / 2;
            semaphore = new SemaphoreSlim(maxParallel, maxParallel);
        }

        public ISplitterHelper Active { get; }

        public IObservable<IParsedDocumentHolder> Load()
        {
            log.Info("Load");
            return helper.AmazonRepository.LoadProductReviews(definition.Product)
                .Select(ProcessReview)
                .Merge();
        }

        private async Task<IParsedDocumentHolder> ProcessReview(AmazonReview review)
        {
            try
            {
                await semaphore.WaitAsync().ConfigureAwait(false);
                var doc = review.CreateDocument();
                if (doc == null)
                {
                    log.Error("Error processing document");
                    return null;
                }

                if (definition.Cached)
                {
                    var parsed = await retryPolicy.ExecuteAsync(() => helper.Cache.GetById(review.Id)).ConfigureAwait(false);
                    return new ParsedDocumentHolder(doc, parsed);
                }

                return new ParsingDocumentHolder(Active.Splitter, new SingleProcessingData(doc));
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
            finally
            {
                semaphore.Release(1);
            }
        }
    }
}
