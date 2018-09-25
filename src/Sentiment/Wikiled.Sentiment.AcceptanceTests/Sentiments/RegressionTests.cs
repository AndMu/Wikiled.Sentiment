using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NLog;
using NUnit.Framework;
using Wikiled.Amazon.Logic;
using Wikiled.Sentiment.AcceptanceTests.Helpers;
using Wikiled.Sentiment.AcceptanceTests.Helpers.Data;
using Wikiled.Sentiment.Analysis.Pipeline;
using Wikiled.Sentiment.Analysis.Processing;
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
            new SentimentTestData("B0002L5R78", 7581, 0, "Total:<7235> Positive:<77.836%> Negative:<71.951%> F1:<0.860> RMSE:1.51"),

            new SentimentTestData("B00002EQCW", 228, 0, "Total:<212> Positive:<85.787%> Negative:<66.667%> F1:<0.911> RMSE:1.34"),
            new SentimentTestData("B000BAX50G", 288, 0, "Total:<266> Positive:<89.020%> Negative:<63.636%> F1:<0.934> RMSE:1.05"),
            new SentimentTestData("B000ERAON2", 440, 0, "Total:<414> Positive:<85.278%> Negative:<75.926%> F1:<0.903> RMSE:1.27"),

            new SentimentTestData("B0026127Y8", 928, 0, "Total:<853> Positive:<69.560%> Negative:<77.778%> F1:<0.809> RMSE:1.49") { Category = ProductCategory.Video },
            new SentimentTestData("B009GN6F5Q", 472, 0, "Total:<390> Positive:<79.487%> Negative:<74.359%> F1:<0.835> RMSE:1.33") { Category = ProductCategory.Video },
            new SentimentTestData("B009CG8YJW", 418, 0, "Total:<345> Positive:<58.278%> Negative:<69.767%> F1:<0.717> RMSE:1.53") { Category = ProductCategory.Video },

            new SentimentTestData("B00004SGFS", 381, 0, "Total:<375> Positive:<94.444%> Negative:<72.727%> F1:<0.958> RMSE:0.95") { Category = ProductCategory.Kitchen },
            new SentimentTestData("B000PYF768", 507, 0, "Total:<479> Positive:<90.433%> Negative:<75.000%> F1:<0.939> RMSE:1.04") { Category = ProductCategory.Kitchen },
            new SentimentTestData("B0000Z6JIW", 297, 0, "Total:<288> Positive:<75.836%> Negative:<94.737%> F1:<0.861> RMSE:1.33") { Category = ProductCategory.Kitchen }
        };

        [TestCaseSource(nameof(testData))]
        public async Task RawSentimentDetection(SentimentTestData data)
        {
            log.Info("RawSentimentDetection: {0}", data);
            TestHelper helper = new TestHelper();
            TestRunner runner = new TestRunner(helper, data);
            await runner.Load().LastOrDefaultAsync();
            TestingClient testing = new TestingClient(new ProcessingPipeline(TaskPoolScheduler.Default, runner.Active), string.Empty);
            testing.DisableAspects = true;
            testing.DisableSvm = true;
            testing.TrackArff = true;
            testing.Init();
            await testing.Process(runner.Load()).LastOrDefaultAsync();
            Assert.AreEqual(data.Errors, testing.Errors);
            Assert.AreEqual(data.Performance, testing.GetPerformanceDescription());
        }
    }
}
