using System.Threading.Tasks;
using NLog;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Amazon.Logic;

namespace Wikiled.Sentiment.AcceptanceTests.Training
{
    [TestFixture]
    public class SentimentTests
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        [TestCase("B00002EQCW", "Total:<213> Positive:<97.47%> Negative:<33.33%> F1:<0.96> RMSE:0.93")]
        public async Task TrainedElectronicsSentimentDetection(string product, string performance)
        {
            log.Info("TrainedElectronicsSentimentDetection: {0} {1}", product, performance);
            var testingClient = await Global.ElectronicBaseLine.Test(product, ProductCategory.Electronics).ConfigureAwait(false);
            Assert.AreEqual(performance, testingClient.GetPerformanceDescription());
        }
    }
}
