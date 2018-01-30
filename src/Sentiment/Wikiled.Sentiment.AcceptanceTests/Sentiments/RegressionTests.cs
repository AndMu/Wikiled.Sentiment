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
    public class RegressionTests
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private static readonly SentimentTestData[] testData =
        {
            new SentimentTestData("NotKnown", performance: "Total:<0> Positive:<0.000%> Negative:<0.000%> F1:<0.000> RMSE:NaN"),
            new SentimentTestData("B0002L5R78", 7581, 0, "Total:<7234> Positive:<75.954%> Negative:<71.680%> F1:<0.848> RMSE:1.56"),

            new SentimentTestData("B00002EQCW", 228, 0, "Total:<212> Positive:<85.279%> Negative:<66.667%> F1:<0.908> RMSE:1.33"),
            new SentimentTestData("B000BAX50G", 288, 0, "Total:<266> Positive:<90.196%> Negative:<63.636%> F1:<0.941> RMSE:1.06"),
            new SentimentTestData("B000ERAON2", 440, 0, "Total:<414> Positive:<83.611%> Negative:<74.074%> F1:<0.892> RMSE:1.30"),

            new SentimentTestData("B00002EQCW", 228, 0, "Total:<212> Positive:<84.772%> Negative:<60.000%> F1:<0.903> RMSE:1.34", false),

            new SentimentTestData("B0026127Y8", 928, 0, "Total:<853> Positive:<69.819%> Negative:<76.543%> F1:<0.811> RMSE:1.47") { Category = ProductCategory.Video },
            new SentimentTestData("B009GN6F5Q", 472, 0, "Total:<390> Positive:<78.755%> Negative:<71.795%> F1:<0.825> RMSE:1.31") { Category = ProductCategory.Video },
            new SentimentTestData("B009CG8YJW", 418, 0, "Total:<345> Positive:<59.272%> Negative:<72.093%> F1:<0.726> RMSE:1.51") { Category = ProductCategory.Video },

            new SentimentTestData("B00004SGFS", 381, 0, "Total:<375> Positive:<93.567%> Negative:<81.818%> F1:<0.958> RMSE:0.95") { Category = ProductCategory.Kitchen },
            new SentimentTestData("B000PYF768", 507, 0, "Total:<479> Positive:<90.433%> Negative:<72.500%> F1:<0.937> RMSE:1.02") { Category = ProductCategory.Kitchen },
            new SentimentTestData("B0000Z6JIW", 297, 0, "Total:<288> Positive:<75.093%> Negative:<94.737%> F1:<0.856> RMSE:1.37") { Category = ProductCategory.Kitchen }
        };

        [TestCaseSource(nameof(testData))]
        public async Task RawSentimentDetection(SentimentTestData data)
        {
            log.Info("RawSentimentDetection: {0}", data);
            TestRunner runner = new TestRunner(TestHelper.Instance, data);
            await runner.Load().LastOrDefaultAsync();
            TestingClient testing = new TestingClient(new ProcessingPipeline(TaskPoolScheduler.Default, runner.Active, runner.Load(), new ParsedReviewManagerFactory()), string.Empty);
            testing.DisableAspects = true;
            testing.DisableSvm = true;
            testing.Init();
            await testing.Process().LastOrDefaultAsync();
            Assert.AreEqual(data.Errors, testing.Errors);
            Assert.AreEqual(data.Performance, testing.GetPerformanceDescription());
        }
    }
}
