using System;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NLog;
using NUnit.Framework;
using Wikiled.Sentiment.AcceptanceTests.Helpers;
using Wikiled.Sentiment.AcceptanceTests.Helpers.Data;
using Wikiled.Sentiment.Analysis.Amazon;
using Wikiled.Sentiment.Analysis.Amazon.Logic;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Pipeline;

namespace Wikiled.Sentiment.AcceptanceTests.Sentiments
{
    [TestFixture]
    public class Word2VecTests
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private static readonly SentimentTestData[] testData =
        {
            new SentimentTestData("B0002L5R78", 7581, 0, "Total:<7234> Positive:<80.79%> Negative:<68.83%> F1:<0.88> RMSE:1.48"),
            new SentimentTestData("B00002EQCW", 228, 0, "Total:<212> Positive:<86.80%> Negative:<73.33%> F1:<0.92> RMSE:1.26"),
            new SentimentTestData("B000BAX50G", 288, 0, "Total:<266> Positive:<96.47%> Negative:<63.64%> F1:<0.97> RMSE:0.87"),
            new SentimentTestData("B000ERAON2", 440, 0, "Total:<414> Positive:<86.67%> Negative:<79.63%> F1:<0.91> RMSE:1.24"),

            new SentimentTestData("B0026127Y8", 928, 0, "Total:<853> Positive:<75.39%> Negative:<77.78%> F1:<0.85> RMSE:1.39") {Category = ProductCategory.Video},
            new SentimentTestData("B009GN6F5Q", 472, 0, "Total:<390> Positive:<85.71%> Negative:<72.65%> F1:<0.87> RMSE:1.26") {Category = ProductCategory.Video},
            new SentimentTestData("B009CG8YJW", 418, 0, "Total:<345> Positive:<69.54%> Negative:<72.09%> F1:<0.80> RMSE:1.35") {Category = ProductCategory.Video},

            new SentimentTestData("B00004SGFS", 381, 0, "Total:<375> Positive:<95.61%> Negative:<72.73%> F1:<0.96> RMSE:0.90") {Category = ProductCategory.Kitchen},
            new SentimentTestData("B000PYF768", 507, 0, "Total:<480> Positive:<90.91%> Negative:<72.50%> F1:<0.94> RMSE:0.94") {Category = ProductCategory.Kitchen},
            new SentimentTestData("B0000Z6JIW", 297, 0, "Total:<288> Positive:<79.55%> Negative:<100.00%> F1:<0.89> RMSE:1.32") {Category = ProductCategory.Kitchen}
        };

        [TearDown]
        public void TearDown()review
        {
            TestHelper.Instance.CachedSplitterHelper.DataLoader.Reset();
        }

        [TestCaseSource(nameof(testData))]
        public async Task SentimentTests(SentimentTestData data)
        {
            log.Info("SentimentTests: {0}", data);
            string file;
            switch (data.Category)
            {
                case ProductCategory.Electronics:
                    file = "Electronics.csv";
                    break;
                case ProductCategory.Video:
                    file = "video.csv";
                    break;
                case ProductCategory.Kitchen:
                    file = "kitchen.csv";
                    break;
                case ProductCategory.Medic:
                case ProductCategory.Games:
                case ProductCategory.Toys:
                case ProductCategory.Book:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var adjuster = new WeightSentimentAdjuster(TestHelper.Instance.CachedSplitterHelper.DataLoader.SentimentDataHolder);
            adjuster.Adjust(Path.Combine(TestContext.CurrentContext.TestDirectory, "Sentiments", file));
            TestRunner runner = new TestRunner(TestHelper.Instance, data);
            TestingClient testing = new TestingClient(new ProcessingPipeline(TaskPoolScheduler.Default, runner.Active, runner.Load()), string.Empty);
            testing.DisableAspects = true;
            testing.DisableSvm = true;
            testing.Init();
            await testing.Process().LastOrDefaultAsync();
            testing.Save(Path.Combine(TestContext.CurrentContext.TestDirectory, "Word2Vec"));
            Assert.AreEqual(data.Performance, testing.GetPerformanceDescription());
            Assert.AreEqual(data.Errors, testing.Errors);
        }
    }
}