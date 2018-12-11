using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Wikiled.Amazon.Logic;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.AcceptanceTests.Helpers;
using Wikiled.Sentiment.AcceptanceTests.Helpers.Data;

namespace Wikiled.Sentiment.AcceptanceTests.Sentiments
{
    [TestFixture]
    public class RegressionTests
    {
         private static readonly ILogger log = ApplicationLogging.CreateLogger<RegressionTests>();

        private static readonly SentimentTestData[] testData =
        {
            new SentimentTestData("NotKnown", performance: "Total:<0> Positive:<0.000%> Negative:<0.000%> F1:<0.000> RMSE:NaN"),
            new SentimentTestData("B0002L5R78", 7581, 0, "Total:<7225> Positive:<79.436%> Negative:<70.054%> F1:<0.869> RMSE:1.48"),

            new SentimentTestData("B00002EQCW", 228, 0, "Total:<209> Positive:<87.629%> Negative:<60.000%> F1:<0.919> RMSE:1.26"),
            new SentimentTestData("B000BAX50G", 288, 0, "Total:<265> Positive:<92.549%> Negative:<60.000%> F1:<0.954> RMSE:0.98"),
            new SentimentTestData("B000ERAON2", 440, 0, "Total:<414> Positive:<86.111%> Negative:<75.926%> F1:<0.908> RMSE:1.24"),

            new SentimentTestData("B0026127Y8", 928, 0, "Total:<853> Positive:<71.762%> Negative:<72.840%> F1:<0.822> RMSE:1.47") { Category = ProductCategory.Video },
            new SentimentTestData("B009GN6F5Q", 472, 0, "Total:<390> Positive:<80.220%> Negative:<70.940%> F1:<0.833> RMSE:1.32") { Category = ProductCategory.Video },
            new SentimentTestData("B009CG8YJW", 418, 0, "Total:<344> Positive:<61.589%> Negative:<64.286%> F1:<0.740> RMSE:1.50") { Category = ProductCategory.Video },

            new SentimentTestData("B00004SGFS", 381, 0, "Total:<375> Positive:<94.737%> Negative:<72.727%> F1:<0.960> RMSE:0.92") { Category = ProductCategory.Kitchen },
            new SentimentTestData("B000PYF768", 507, 0, "Total:<479> Positive:<91.116%> Negative:<65.000%> F1:<0.938> RMSE:1.00") { Category = ProductCategory.Kitchen },
            new SentimentTestData("B0000Z6JIW", 297, 0, "Total:<288> Positive:<78.439%> Negative:<84.211%> F1:<0.874> RMSE:1.30") { Category = ProductCategory.Kitchen }
        };

        [TestCaseSource(nameof(testData))]
        public async Task RawSentimentDetection(SentimentTestData data)
        {
            log.LogInformation("RawSentimentDetection: {0}", data);
            TestHelper helper = new TestHelper();
            TestRunner runner = new TestRunner(helper, data);
            await runner.Load().LastOrDefaultAsync();
            var testing = runner.Active.GetTesting();
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
