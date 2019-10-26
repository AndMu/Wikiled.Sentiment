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

        [TestCase("B00002EQCW", "Total:<215> Positive:<92.000%> Negative:<80.000%> F1:<0.951> RMSE:1.10")]
        [TestCase("B0026127Y8", "Total:<854> Positive:<80.207%> Negative:<58.025%> F1:<0.869> RMSE:1.37")]
        public async Task TestElectronics(string product, string performance)
        {
            log.LogInformation("TestElectronics: {0} {1}", product, performance);
            var testingClient = await Global.ElectronicBaseLine.Test(product, ProductCategory.Electronics).ConfigureAwait(false);
            Assert.AreEqual(performance, testingClient.GetPerformanceDescription());
        }


        [TestCase("B0002L5R78", "Total:<7270> Positive:<80.171%> Negative:<42.760%> F1:<0.859> RMSE:1.609")]
        public async Task TestVideo(string product, string performance)
        {
            log.LogInformation("TestVideo: {0} {1}", product, performance);
            var testingClient = await Global.VideoBaseLine.Test(product, ProductCategory.Video).ConfigureAwait(false);
            Assert.AreEqual(performance, testingClient.GetPerformanceDescription());
        }
    }
}
