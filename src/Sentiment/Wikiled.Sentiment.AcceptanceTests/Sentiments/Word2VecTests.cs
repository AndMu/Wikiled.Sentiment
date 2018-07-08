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
            new SentimentTestData("B0002L5R78", 7581, 0, "Total:<7236> Positive:<83.749%> Negative:<63.415%> F1:<0.891> RMSE:1.43"),
            new SentimentTestData("B00002EQCW", 228, 0, "Total:<212> Positive:<85.279%> Negative:<80.000%> F1:<0.913> RMSE:1.34"),
            new SentimentTestData("B000BAX50G", 288, 0, "Total:<266> Positive:<96.078%> Negative:<63.636%> F1:<0.972> RMSE:0.89"),
            new SentimentTestData("B000ERAON2", 440, 0, "Total:<414> Positive:<86.389%> Negative:<75.926%> F1:<0.909> RMSE:1.26"),

            new SentimentTestData("B0026127Y8", 928, 0, "Total:<853> Positive:<78.109%> Negative:<79.012%> F1:<0.866> RMSE:1.35") { Category = ProductCategory.Video },
            new SentimentTestData("B009GN6F5Q", 472, 0, "Total:<390> Positive:<88.645%> Negative:<69.231%> F1:<0.878> RMSE:1.25") { Category = ProductCategory.Video },
            new SentimentTestData("B009CG8YJW", 418, 0, "Total:<345> Positive:<82.781%> Negative:<53.488%> F1:<0.874> RMSE:1.19") { Category = ProductCategory.Video },

            new SentimentTestData("B00004SGFS", 381, 0, "Total:<375> Positive:<96.491%> Negative:<66.667%> F1:<0.966> RMSE:0.91") { Category = ProductCategory.Kitchen },
            new SentimentTestData("B000PYF768", 507, 0, "Total:<480> Positive:<92.273%> Negative:<67.500%> F1:<0.945> RMSE:0.97") { Category = ProductCategory.Kitchen },
            new SentimentTestData("B0000Z6JIW", 297, 0, "Total:<288> Positive:<83.271%> Negative:<100.000%> F1:<0.909> RMSE:1.28") { Category = ProductCategory.Kitchen }
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
            TestingClient testing = new TestingClient(new ProcessingPipeline(TaskPoolScheduler.Default, runner.Active, new ParsedReviewManagerFactory()), string.Empty);
            testing.DisableAspects = true;
            testing.DisableSvm = true;
            testing.TrackArff = true;
            testing.Init();
            await testing.Process(runner.Load()).LastOrDefaultAsync();
            testing.Save(Path.Combine(TestContext.CurrentContext.TestDirectory, "Word2Vec"));
            Assert.AreEqual(data.Performance, testing.GetPerformanceDescription());
            Assert.AreEqual(data.Errors, testing.Errors);
        }
    }
}