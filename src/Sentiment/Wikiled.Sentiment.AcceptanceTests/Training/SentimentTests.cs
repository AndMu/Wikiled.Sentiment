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

        [TestCase("B00002EQCW", "Total:<214> Positive:<90.955%> Negative:<80.000%> F1:<0.945> RMSE:1.14")]
        [TestCase("B0026127Y8", "Total:<854> Positive:<78.396%> Negative:<61.728%> F1:<0.860> RMSE:1.40")]
        public async Task TestElectronics(string product, string performance)
        {
            log.LogInformation("TestElectronics: {0} {1}", product, performance);
            var testingClient = await Global.ElectronicBaseLine.Test(product, ProductCategory.Electronics).ConfigureAwait(false);
            Assert.AreEqual(performance, testingClient.GetPerformanceDescription());
        }


        [TestCase("B0002L5R78", "Total:<7266> Positive:<80.726%> Negative:<40.866%> F1:<0.861> RMSE:1.59")]
        public async Task TestVideo(string product, string performance)
        {
            log.LogInformation("TestVideo: {0} {1}", product, performance);
            var testingClient = await Global.VideoBaseLine.Test(product, ProductCategory.Video).ConfigureAwait(false);
            Assert.AreEqual(performance, testingClient.GetPerformanceDescription());
        }
    }
}
