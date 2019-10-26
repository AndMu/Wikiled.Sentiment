using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Wikiled.Amazon.Logic;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.AcceptanceTests.Helpers;
using Wikiled.Sentiment.AcceptanceTests.Helpers.Data;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.AcceptanceTests.Sentiments
{
    [TestFixture]
    public class Word2VecTests
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<Word2VecTests>();

        private static readonly SentimentTestData[] testData =
        {
            new SentimentTestData("B0002L5R78", 7581, 0, "Total:<7207> Positive:<80.590%> Negative:<61.549%> F1:<0.871> RMSE:1.57"),
            new SentimentTestData("B00002EQCW", 228, 0, "Total:<212> Positive:<81.726%> Negative:<86.667%> F1:<0.894> RMSE:1.48"),
            new SentimentTestData("B000BAX50G", 288, 0, "Total:<265> Positive:<96.078%> Negative:<60.000%> F1:<0.972> RMSE:0.92"),
            new SentimentTestData("B000ERAON2", 440, 0, "Total:<413> Positive:<84.401%> Negative:<70.370%> F1:<0.894> RMSE:1.41"),

            new SentimentTestData("B0026127Y8", 928, 0, "Total:<849> Positive:<78.776%> Negative:<71.605%> F1:<0.867> RMSE:1.39") { Category = ProductCategory.Video },
            new SentimentTestData("B009GN6F5Q", 472, 0, "Total:<388> Positive:<87.132%> Negative:<66.379%> F1:<0.865> RMSE:1.33") { Category = ProductCategory.Video },
            new SentimentTestData("B009CG8YJW", 418, 0, "Total:<345> Positive:<87.748%> Negative:<58.140%> F1:<0.906> RMSE:1.15") { Category = ProductCategory.Video },

            new SentimentTestData("B00004SGFS", 381, 0, "Total:<363> Positive:<82.779%> Negative:<81.250%> F1:<0.897> RMSE:1.41") { Category = ProductCategory.Kitchen },
            new SentimentTestData("B000PYF768", 507, 0, "Total:<475> Positive:<83.678%> Negative:<70.000%> F1:<0.898> RMSE:1.35") { Category = ProductCategory.Kitchen },
            new SentimentTestData("B0000Z6JIW", 297, 0, "Total:<279> Positive:<87.739%> Negative:<55.556%> F1:<0.920> RMSE:1.19") { Category = ProductCategory.Kitchen }
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
            log.LogInformation("SentimentTests: {0}", data);
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

            ISentimentDataHolder holder = SentimentDataHolder.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, "Sentiments", file));
            var runner = new TestRunner(TestHelper.Instance, data);
            Analysis.Processing.ITestingClient testing = runner.Active.GetTesting();
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