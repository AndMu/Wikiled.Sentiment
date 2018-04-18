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
            new SentimentTestData("B0002L5R78", 7581, 0, "Total:<7236> Positive:<76.547%> Negative:<72.493%> F1:<0.852> RMSE:1.55"),

            new SentimentTestData("B00002EQCW", 228, 0, "Total:<212> Positive:<84.772%> Negative:<60.000%> F1:<0.903> RMSE:1.34"),
            new SentimentTestData("B000BAX50G", 288, 0, "Total:<266> Positive:<89.020%> Negative:<63.636%> F1:<0.934> RMSE:1.07"),
            new SentimentTestData("B000ERAON2", 440, 0, "Total:<414> Positive:<82.222%> Negative:<75.926%> F1:<0.885> RMSE:1.35"),

            new SentimentTestData("B0026127Y8", 928, 0, "Total:<853> Positive:<69.171%> Negative:<76.543%> F1:<0.806> RMSE:1.49") { Category = ProductCategory.Video },
            new SentimentTestData("B009GN6F5Q", 472, 0, "Total:<390> Positive:<78.755%> Negative:<73.504%> F1:<0.829> RMSE:1.33") { Category = ProductCategory.Video },
            new SentimentTestData("B009CG8YJW", 418, 0, "Total:<345> Positive:<57.947%> Negative:<69.767%> F1:<0.714> RMSE:1.53") { Category = ProductCategory.Video },

            new SentimentTestData("B00004SGFS", 381, 0, "Total:<375> Positive:<93.275%> Negative:<78.788%> F1:<0.955> RMSE:0.96") { Category = ProductCategory.Kitchen },
            new SentimentTestData("B000PYF768", 507, 0, "Total:<479> Positive:<90.433%> Negative:<72.500%> F1:<0.937> RMSE:1.03") { Category = ProductCategory.Kitchen },
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
            //var result = await testing.Process().ToArray();
            //result = result.OrderBy(item => item.Processed.Id).ToArray();
            //var text = result.Select(item => item.Adjustment.Rating.RawRating?.ToString()).Aggregate((one, two) => one + Environment.NewLine + two);
            Assert.AreEqual(data.Errors, testing.Errors);
            Assert.AreEqual(data.Performance, testing.GetPerformanceDescription());
        }
    }
}
