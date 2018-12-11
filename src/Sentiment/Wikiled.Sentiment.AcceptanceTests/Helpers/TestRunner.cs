using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.Amazon.Logic;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.AcceptanceTests.Helpers.Data;
using Wikiled.Sentiment.Analysis.Containers;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.AcceptanceTests.Helpers
{
    public class TestRunner
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<TestRunner>();

        private readonly SentimentTestData definition;

        private readonly TestHelper helper;

        private readonly SemaphoreSlim semaphore;

        public TestRunner(TestHelper helper, SentimentTestData definition)
        {
            this.helper = helper;
            this.definition = definition;
            Active = helper.ContainerHelper;
            int maxParallel = Environment.ProcessorCount / 2;
            semaphore = new SemaphoreSlim(maxParallel, maxParallel);
        }

        public ISessionContainer Active { get; }

        public IObservable<IParsedDocumentHolder> Load()
        {
            log.LogInformation("Load");
            return helper.AmazonRepository.LoadProductReviews(definition.Product)
                .Select(ProcessReview)
                .Merge();
        }

        private async Task<IParsedDocumentHolder> ProcessReview(AmazonReview review)
        {
            try
            {
                await semaphore.WaitAsync().ConfigureAwait(false);
                Wikiled.Text.Analysis.Structure.Document doc = review.CreateDocument();
                if (doc == null)
                {
                    log.LogError("Error processing document");
                    return null;
                }

                return new ParsingDocumentHolder(Active.Resolve<ITextSplitter>(), doc);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error");
                return null;
            }
            finally
            {
                semaphore.Release(1);
            }
        }
    }
}
