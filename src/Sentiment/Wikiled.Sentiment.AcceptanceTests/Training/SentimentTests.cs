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

        [TestCase("B00002EQCW", "Total:<214> Positive:<96.000%> Negative:<71.429%> F1:<0.970> RMSE:0.94")]
        [TestCase("B0026127Y8", "Total:<854> Positive:<88.875%> Negative:<39.506%> F1:<0.911> RMSE:1.19")]
        public async Task TestElectronics(string product, string performance)
        {
            log.Info("TestElectronics: {0} {1}", product, performance);
            var testingClient = await Global.ElectronicBaseLine.Test(product, ProductCategory.Electronics).ConfigureAwait(false);
            Assert.AreEqual(performance, testingClient.GetPerformanceDescription());
        }


        [TestCase("B0002L5R78", "Total:<7254> Positive:<89.538%> Negative:<33.878%> F1:<0.909> RMSE:1.29")]
        public async Task TestVideo(string product, string performance)
        {
            log.Info("TestVideo: {0} {1}", product, performance);
            var testingClient = await Global.VideoBaseLine.Test(product, ProductCategory.Video).ConfigureAwait(false);
            Assert.AreEqual(performance, testingClient.GetPerformanceDescription());
        }
    }
}
