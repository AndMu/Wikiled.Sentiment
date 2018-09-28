using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NLog;
using NUnit.Framework;
using Wikiled.Amazon.Logic;
using Wikiled.Sentiment.AcceptanceTests.Helpers;
using Wikiled.Sentiment.AcceptanceTests.Helpers.Data;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.AcceptanceTests.Sentiments
{
    [TestFixture]
    public class Word2VecTests
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private static readonly SentimentTestData[] testData =
        {
            new SentimentTestData("B0002L5R78", 7581, 0, "Total:<7206> Positive:<80.757%> Negative:<61.685%> F1:<0.873> RMSE:1.57"),
            new SentimentTestData("B00002EQCW", 228, 0, "Total:<212> Positive:<81.726%> Negative:<86.667%> F1:<0.894> RMSE:1.47"),
            new SentimentTestData("B000BAX50G", 288, 0, "Total:<265> Positive:<96.471%> Negative:<60.000%> F1:<0.974> RMSE:0.90"),
            new SentimentTestData("B000ERAON2", 440, 0, "Total:<413> Positive:<84.401%> Negative:<72.222%> F1:<0.895> RMSE:1.40"),

            new SentimentTestData("B0026127Y8", 928, 0, "Total:<849> Positive:<79.948%> Negative:<71.605%> F1:<0.874> RMSE:1.38") { Category = ProductCategory.Video },
            new SentimentTestData("B009GN6F5Q", 472, 0, "Total:<388> Positive:<87.500%> Negative:<65.517%> F1:<0.865> RMSE:1.33") { Category = ProductCategory.Video },
            new SentimentTestData("B009CG8YJW", 418, 0, "Total:<345> Positive:<87.086%> Negative:<53.488%> F1:<0.899> RMSE:1.15") { Category = ProductCategory.Video },

            new SentimentTestData("B00004SGFS", 381, 0, "Total:<366> Positive:<85.928%> Negative:<81.250%> F1:<0.915> RMSE:1.33") { Category = ProductCategory.Kitchen },
            new SentimentTestData("B000PYF768", 507, 0, "Total:<475> Positive:<82.989%> Negative:<62.500%> F1:<0.890> RMSE:1.3") { Category = ProductCategory.Kitchen },
            new SentimentTestData("B0000Z6JIW", 297, 0, "Total:<279> Positive:<89.272%> Negative:<55.556%> F1:<0.928> RMSE:1.16") { Category = ProductCategory.Kitchen }
        };

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

            var holder = SentimentDataHolder.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, "Sentiments", file));
            TestRunner runner = new TestRunner(TestHelper.Instance, data);
            var testing = runner.Active.GetTesting();
            runner.Active.Context.Lexicon = holder;
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