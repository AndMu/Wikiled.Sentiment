using System;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NLog;
using NUnit.Framework;
using Wikiled.Amazon.Logic;
using Wikiled.Sentiment.AcceptanceTests.Helpers;
using Wikiled.Sentiment.AcceptanceTests.Helpers.Data;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Pipeline;
using Wikiled.Sentiment.Text.NLP;

namespace Wikiled.Sentiment.AcceptanceTests.Sentiments
{
    [TestFixture]
    public class Word2VecTests
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private static readonly SentimentTestData[] testData =
        {
            new SentimentTestData("B0002L5R78", 7581, 0, "Total:<7177> Positive:<91.474%> Negative:<50.000%> F1:<0.928> RMSE:1.43"),
            new SentimentTestData("B00002EQCW", 228, 0, "Total:<208> Positive:<90.206%> Negative:<57.143%> F1:<0.933> RMSE:1.34"),
            new SentimentTestData("B000BAX50G", 288, 0, "Total:<266> Positive:<97.647%> Negative:<45.455%> F1:<0.976> RMSE:0.88"),
            new SentimentTestData("B000ERAON2", 440, 0, "Total:<412> Positive:<90.808%> Negative:<67.925%> F1:<0.929> RMSE:1.18"),

            new SentimentTestData("B0026127Y8", 928, 0, "Total:<853> Positive:<84.326%> Negative:<59.259%> F1:<0.894> RMSE:1.29") { Category = ProductCategory.Video },
            new SentimentTestData("B009GN6F5Q", 472, 0, "Total:<389> Positive:<93.040%> Negative:<57.759%> F1:<0.882>") { Category = ProductCategory.Video },
            new SentimentTestData("B009CG8YJW", 418, 0, "Total:<344> Positive:<90.066%> Negative:<47.619%> F1:<0.913> RMSE:1.17") { Category = ProductCategory.Video },

            new SentimentTestData("B00004SGFS", 381, 0, "Total:<375> Positive:<97.953%> Negative:<48.485%> F1:<0.965> RMSE:0.88") { Category = ProductCategory.Kitchen },
            new SentimentTestData("B000PYF768", 507, 0, "Total:<480> Positive:<96.136%> Negative:<45.000%> F1:<0.956> RMSE:0.89") { Category = ProductCategory.Kitchen },
            new SentimentTestData("B0000Z6JIW", 297, 0, "Total:<288> Positive:<87.732%> Negative:<89.474%> F1:<0.931> RMSE:1.16") { Category = ProductCategory.Kitchen }
        };

        [TearDown]
        public void TearDown()
        {
            TestHelper.Instance.SplitterHelper.DataLoader.Reset();
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

            var adjuster = new WeightSentimentAdjuster(TestHelper.Instance.SplitterHelper.DataLoader.SentimentDataHolder);
            adjuster.Adjust(Path.Combine(TestContext.CurrentContext.TestDirectory, "Sentiments", file));
            TestRunner runner = new TestRunner(TestHelper.Instance, data);
            TestingClient testing = new TestingClient(new ProcessingPipeline(TaskPoolScheduler.Default, runner.Active, runner.Load(), new ParsedReviewManagerFactory()), string.Empty);
            testing.DisableAspects = true;
            testing.DisableSvm = true;
            testing.TrackArff = true;
            testing.Init();
            await testing.Process().LastOrDefaultAsync();
            testing.Save(Path.Combine(TestContext.CurrentContext.TestDirectory, "Word2Vec"));
            Assert.AreEqual(data.Performance, testing.GetPerformanceDescription());
            Assert.AreEqual(data.Errors, testing.Errors);
        }
    }
}