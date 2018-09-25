using System;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using NLog;
using NUnit.Framework;
using Wikiled.Sentiment.AcceptanceTests.Helpers;
using Wikiled.Sentiment.AcceptanceTests.Helpers.Data;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.NLP;

namespace Wikiled.Sentiment.AcceptanceTests.Aspects
{
    [TestFixture]
    public class AspectsTests
    {
        private static readonly SentimentAspectData[] aspectData =
            {
                new SentimentAspectData(
                    new SentimentTestData("B0002L5R78", 7581),
                    new TopItems { Total = 10, Items = new[] { "new" } }, // attributes 
                    new TopItems { Total = 10, Items = new[] { "cable" } }), // features
                new SentimentAspectData(
                    new SentimentTestData("B00002EQCW", 228),
                    new TopItems { Total = 10, Items = new[] { "small" } }, // attributes 
                    new TopItems { Total = 10, Items = new[] { "switch" } }) // features
            };

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        [TestCaseSource(nameof(aspectData))]
        public async Task ProductTest(SentimentAspectData data)
        {
            log.Info("ProductTest: {0}", data);
            MainAspectHandler aspectHandler = new MainAspectHandler(new AspectContextFactory());
            TestRunner runner = new TestRunner(TestHelper.Instance, data.Sentiment);

            SemaphoreSlim semaphore = new SemaphoreSlim(Environment.ProcessorCount / 2, Environment.ProcessorCount / 2);
            var result = runner.Load().ObserveOn(TaskPoolScheduler.Default).Select(review => ProcessItem(semaphore, aspectHandler, review)).Merge();

            await result;

            var serializer = TestHelper.Instance.ContainerHelper.Container.Resolve<IAspectSerializer>();
            serializer.Serialize(aspectHandler).Save(Path.Combine(TestContext.CurrentContext.TestDirectory, data.Sentiment.Product + ".xml"));

            var features = aspectHandler.GetFeatures(10).ToArray();
            var attributes = aspectHandler.GetAttributes(10).ToArray();
            for (int i = 0; i < data.Features.Items.Length; i++)
            {
                Assert.IsTrue(features.Any(item => string.Compare(item.Text, data.Features.Items[i], StringComparison.OrdinalIgnoreCase) == 0));
            }

            for (int i = 0; i < data.Attributes.Items.Length; i++)
            {
                Assert.IsTrue(attributes.Any(item => string.Compare(item.Text, data.Attributes.Items[i], StringComparison.OrdinalIgnoreCase) == 0));
            }
        }

        private static async Task<IParsedDocumentHolder> ProcessItem(SemaphoreSlim semaphore, MainAspectHandler aspectHandler, IParsedDocumentHolder review)
        {
            try
            {
                await semaphore.WaitAsync().ConfigureAwait(false);
                var parsedDoc = await review.GetParsed().ConfigureAwait(false);
                var parseReview = TestHelper.Instance.ContainerHelper.Container.Resolve<IParsedReviewManagerFactory>().Resolve(parsedDoc).Create();
                aspectHandler.Process(parseReview);
                return review;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
