using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Wikiled.Amazon.Logic;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.AcceptanceTests.Helpers;
using Wikiled.Sentiment.AcceptanceTests.Helpers.Data;
using Wikiled.Sentiment.Analysis.Pipeline.Persistency;

namespace Wikiled.Sentiment.AcceptanceTests.Sentiments
{
    [TestFixture]
    public class RegressionTests
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<RegressionTests>();

        private static readonly SentimentTestData[] testData =
        {
            new SentimentTestData("NotKnown", performance: "Total:<0> Positive:<0.000%> Negative:<0.000%> F1:<0.000> RMSE:NaN"),
            new SentimentTestData("B0002L5R78", 7581, 0, "Total:<7237> Positive:<77.504%> Negative:<71.545%> F1:<0.858> RMSE:1.52"),

            new SentimentTestData("B00002EQCW", 228, 0, "Total:<212> Positive:<86.802%> Negative:<66.667%> F1:<0.917> RMSE:1.34"),
            new SentimentTestData("B000BAX50G", 288, 0, "Total:<266> Positive:<90.980%> Negative:<63.636%> F1:<0.945> RMSE:1.02"),
            new SentimentTestData("B000ERAON2", 440, 0, "Total:<414> Positive:<85.833%> Negative:<74.074%> F1:<0.905> RMSE:1.24"),

            new SentimentTestData("B0026127Y8", 928, 0, "Total:<853> Positive:<70.596%> Negative:<74.074%> F1:<0.815> RMSE:1.47") { Category = ProductCategory.Video },
            new SentimentTestData("B009GN6F5Q", 472, 0, "Total:<390> Positive:<79.487%> Negative:<75.214%> F1:<0.836> RMSE:1.30") { Category = ProductCategory.Video },
            new SentimentTestData("B009CG8YJW", 418, 0, "Total:<345> Positive:<59.603%> Negative:<72.093%> F1:<0.729> RMSE:1.50") { Category = ProductCategory.Video },

            new SentimentTestData("B00004SGFS", 381, 0, "Total:<375> Positive:<94.737%> Negative:<75.758%> F1:<0.961> RMSE:0.94") { Category = ProductCategory.Kitchen },
            new SentimentTestData("B000PYF768", 507, 0, "Total:<479> Positive:<89.977%> Negative:<75.000%> F1:<0.936> RMSE:1.02") { Category = ProductCategory.Kitchen },
            new SentimentTestData("B0000Z6JIW", 297, 0, "Total:<288> Positive:<77.323%> Negative:<94.737%> F1:<0.870> RMSE:1.34") { Category = ProductCategory.Kitchen }
        };

        [TestCaseSource(nameof(testData))]
        public async Task RawSentimentDetection(SentimentTestData data)
        {
            log.LogInformation("RawSentimentDetection: {0}", data);
            var helper = new TestHelper();
            var runner = new TestRunner(helper, data);
            await runner.Load().LastOrDefaultAsync();
            var testing = runner.Active.GetTesting();
            testing.DisableAspects = true;
            testing.DisableSvm = true;
            testing.TrackArff = true;
            testing.Init();
            using var persistency = runner.Active.Resolve<IPipelinePersistency>();
            persistency.Start(Path.Combine(TestContext.CurrentContext.TestDirectory, "Results", data.Product));
            persistency.Debug = true;
            await testing.Process(runner.Load()).ForEachAsync(
                item =>
                {
                    //persistency.Save(item);
                });
            Assert.AreEqual(data.Errors, testing.Errors);
            Assert.AreEqual(data.Performance, testing.GetPerformanceDescription());
        }
    }
}
