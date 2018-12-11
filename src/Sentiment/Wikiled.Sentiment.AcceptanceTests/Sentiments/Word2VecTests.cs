using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
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
            new SentimentTestData("B0002L5R78", 7581, 0, "Total:<7184> Positive:<82.690%> Negative:<59.644%> F1:<0.883> RMSE:1.52"),
            new SentimentTestData("B00002EQCW", 228, 0, "Total:<208> Positive:<83.420%> Negative:<86.667%> F1:<0.904> RMSE:1.41"),
            new SentimentTestData("B000BAX50G", 288, 0, "Total:<263> Positive:<98.031%> Negative:<66.667%> F1:<0.984> RMSE:0.83"),
            new SentimentTestData("B000ERAON2", 440, 0, "Total:<412> Positive:<85.475%> Negative:<70.370%> F1:<0.900> RMSE:1.37"),

            new SentimentTestData("B0026127Y8", 928, 0, "Total:<848> Positive:<82.008%> Negative:<66.667%> F1:<0.884> RMSE:1.35") { Category = ProductCategory.Video },
            new SentimentTestData("B009GN6F5Q", 472, 0, "Total:<387> Positive:<89.668%> Negative:<57.759%> F1:<0.863> RMSE:1.35") { Category = ProductCategory.Video },
            new SentimentTestData("B009CG8YJW", 418, 0, "Total:<344> Positive:<90.066%> Negative:<47.619%> F1:<0.913> RMSE:1.14") { Category = ProductCategory.Video },

            new SentimentTestData("B00004SGFS", 381, 0, "Total:<361> Positive:<87.538%> Negative:<62.500%> F1:<0.916> RMSE:1.30") { Category = ProductCategory.Kitchen },
            new SentimentTestData("B000PYF768", 507, 0, "Total:<473> Positive:<90.300%> Negative:<52.500%> F1:<0.928> RMSE:1.20") { Category = ProductCategory.Kitchen },
            new SentimentTestData("B0000Z6JIW", 297, 0, "Total:<279> Positive:<91.954%> Negative:<44.444%> F1:<0.939> RMSE:1.05") { Category = ProductCategory.Kitchen }
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