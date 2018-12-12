using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.AcceptanceTests.Helpers;
using Wikiled.Sentiment.AcceptanceTests.Helpers.Data;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Text.Analysis.Structure;

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

        private static readonly ILogger log = ApplicationLogging.CreateLogger<AspectsTests>();

        [SetUp]
        public void Setup()
        {
            TestHelper.Instance.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            TestHelper.Instance.Reset();
        }

        [TestCaseSource(nameof(aspectData))]
        public async Task ProductTest(SentimentAspectData data)
        {
            log.LogInformation("ProductTest: {0}", data);
            var aspectHandler = new MainAspectHandler(new AspectContextFactory());
            var runner = new TestRunner(TestHelper.Instance, data.Sentiment);

            var semaphore = new SemaphoreSlim(Environment.ProcessorCount / 2, Environment.ProcessorCount / 2);
            IObservable<IParsedDocumentHolder> result = runner.Load().ObserveOn(TaskPoolScheduler.Default).Select(review => ProcessItem(semaphore, aspectHandler, review)).Merge();

            await result;

            IAspectSerializer serializer = TestHelper.Instance.ContainerHelper.Resolve<IAspectSerializer>();
            serializer.Serialize(aspectHandler).Save(Path.Combine(TestContext.CurrentContext.TestDirectory, data.Sentiment.Product + ".xml"));

            Text.Words.IWordItem[] features = aspectHandler.GetFeatures(10).ToArray();
            Text.Words.IWordItem[] attributes = aspectHandler.GetAttributes(10).ToArray();
            for (var i = 0; i < data.Features.Items.Length; i++)
            {
                Assert.IsTrue(features.Any(item => string.Compare(item.Text, data.Features.Items[i], StringComparison.OrdinalIgnoreCase) == 0));
            }

            for (var i = 0; i < data.Attributes.Items.Length; i++)
            {
                Assert.IsTrue(attributes.Any(item => string.Compare(item.Text, data.Attributes.Items[i], StringComparison.OrdinalIgnoreCase) == 0));
            }
        }

        private static async Task<IParsedDocumentHolder> ProcessItem(SemaphoreSlim semaphore, MainAspectHandler aspectHandler, IParsedDocumentHolder review)
        {
            try
            {
                await semaphore.WaitAsync().ConfigureAwait(false);
                Document parsedDoc = await review.GetParsed().ConfigureAwait(false);
                Text.Data.IParsedReview parseReview = TestHelper.Instance.ContainerHelper.Resolve<Func<Document, IParsedReviewManager>>()(parsedDoc).Create();
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
