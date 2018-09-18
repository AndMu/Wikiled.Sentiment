using System.Threading.Tasks;
using NLog;
using NUnit.Framework;
using Wikiled.Amazon.Logic;

namespace Wikiled.Sentiment.AcceptanceTests.Training
{
    [TestFixture]
    public class SentimentTests
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        [TestCase("B00002EQCW", "Total:<215> Positive:<98.000%> Negative:<53.333%> F1:<0.973> RMSE:0.89")]
        [TestCase("B0026127Y8", "Total:<854> Positive:<90.944%> Negative:<43.210%> F1:<0.924> RMSE:1.12")]
        public async Task TestElectronics(string product, string performance)
        {
            log.Info("TestElectronics: {0} {1}", product, performance);
            var testingClient = await Global.ElectronicBaseLine.Test(product, ProductCategory.Electronics).ConfigureAwait(false);
            Assert.AreEqual(performance, testingClient.GetPerformanceDescription());
        }


        [TestCase("B0002L5R78", "Total:<7275>Positive:<91.616%> Negative:<31.529%> F1:<0.919> RMSE:1.22")]
        public async Task TestVideo(string product, string performance)
        {
            log.Info("TestVideo: {0} {1}", product, performance);
            var testingClient = await Global.VideoBaseLine.Test(product, ProductCategory.Video).ConfigureAwait(false);
            Assert.AreEqual(performance, testingClient.GetPerformanceDescription());
        }
    }
}
