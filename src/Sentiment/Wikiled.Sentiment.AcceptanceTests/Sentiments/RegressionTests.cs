using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NLog;
using NUnit.Framework;
using Wikiled.Sentiment.AcceptanceTests.Helpers;
using Wikiled.Sentiment.AcceptanceTests.Helpers.Data;
using Wikiled.Sentiment.Analysis.Amazon.Logic;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Pipeline;

namespace Wikiled.Sentiment.AcceptanceTests.Sentiments
{
    [TestFixture]
    public class RegressionTests
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private static readonly SentimentTestData[] testData =
        {
            new SentimentTestData("NotKnown", performance: "Total:<0> Positive:<0.00%> Negative:<0.00%> F1:<0.00> RMSE:NaN"),
            new SentimentTestData("B0002L5R78", 7581, 0, "Total:<7234> Positive:<75.95%> Negative:<71.68%> F1:<0.85> RMSE:1.56"),

            new SentimentTestData("B00002EQCW", 228, 0, "Total:<212> Positive:<85.28%> Negative:<66.67%> F1:<0.91> RMSE:1.33"),
            new SentimentTestData("B000BAX50G", 288, 0, "Total:<266> Positive:<90.20%> Negative:<63.64%> F1:<0.94> RMSE:1.06"),
            new SentimentTestData("B000ERAON2", 440, 0, "Total:<414> Positive:<83.61%> Negative:<74.07%> F1:<0.89> RMSE:1.30"),

            new SentimentTestData("B00002EQCW", 228, 0, "Total:<212> Positive:<84.77%> Negative:<60.00%> F1:<0.90> RMSE:1.34", false),

            new SentimentTestData("B0026127Y8", 928, 0, "Total:<853> Positive:<69.82%> Negative:<76.54%> F1:<0.81> RMSE:1.47") {Category = ProductCategory.Video},
            new SentimentTestData("B009GN6F5Q", 472, 0, "Total:<390> Positive:<78.75%> Negative:<71.79%> F1:<0.83> RMSE:1.31") {Category = ProductCategory.Video},
            new SentimentTestData("B009CG8YJW", 418, 0, "Total:<345> Positive:<59.27%> Negative:<72.09%> F1:<0.73> RMSE:1.51") {Category = ProductCategory.Video},

            new SentimentTestData("B00004SGFS", 381, 0, "Total:<375> Positive:<93.57%> Negative:<81.82%> F1:<0.96> RMSE:0.95") {Category = ProductCategory.Kitchen},
            new SentimentTestData("B000PYF768", 507, 0, "Total:<479> Positive:<90.43%> Negative:<72.50%> F1:<0.94> RMSE:1.02") {Category = ProductCategory.Kitchen},
            new SentimentTestData("B0000Z6JIW", 297, 0, "Total:<288> Positive:<75.09%> Negative:<94.74%> F1:<0.86> RMSE:1.37") {Category = ProductCategory.Kitchen}
        };

        [TestCaseSource(nameof(testData))]
        public async Task RawSentimentDetection(SentimentTestData data)
        {
            log.Info("RawSentimentDetection: {0}", data);
            TestRunner runner = new TestRunner(TestHelper.Instance, data);
            await runner.Load().LastOrDefaultAsync();
            TestingClient testing = new TestingClient(new ProcessingPipeline(TaskPoolScheduler.Default, runner.Active, runner.Load()), string.Empty);
            testing.DisableAspects = true;
            testing.DisableSvm = true;
            testing.Init();
            await testing.Process().LastOrDefaultAsync();
            Assert.AreEqual(data.Errors, testing.Errors);
            Assert.AreEqual(data.Performance, testing.GetPerformanceDescription());
        }
    }
}
