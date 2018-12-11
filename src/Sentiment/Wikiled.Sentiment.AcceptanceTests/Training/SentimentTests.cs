using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Wikiled.Amazon.Logic;
using Wikiled.Common.Logging;

namespace Wikiled.Sentiment.AcceptanceTests.Training
{
    [TestFixture]
    public class SentimentTests
    {
         private static readonly ILogger log = ApplicationLogging.CreateLogger<SentimentTests>();

        [TestCase("B00002EQCW", "Total:<215> Positive:<98.000%> Negative:<46.667%> F1:<0.970> RMSE:0.90")]
        [TestCase("B0026127Y8", "Total:<854> Positive:<91.721%> Negative:<43.210%> F1:<0.928> RMSE:1.11")]
        public async Task TestElectronics(string product, string performance)
        {
            log.LogInformation("TestElectronics: {0} {1}", product, performance);
            var testingClient = await Global.ElectronicBaseLine.Test(product, ProductCategory.Electronics).ConfigureAwait(false);
            Assert.AreEqual(performance, testingClient.GetPerformanceDescription());
        }


        [TestCase("B0002L5R78", "Total:<7274> Positive:<91.079%> Negative:<31.800%> F1:<0.916> RMSE:1.25")]
        public async Task TestVideo(string product, string performance)
        {
            log.LogInformation("TestVideo: {0} {1}", product, performance);
            var testingClient = await Global.VideoBaseLine.Test(product, ProductCategory.Video).ConfigureAwait(false);
            Assert.AreEqual(performance, testingClient.GetPerformanceDescription());
        }
    }
}
