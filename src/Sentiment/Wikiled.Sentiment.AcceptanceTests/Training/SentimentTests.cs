using System;
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

        [TestCase("B00002EQCW", 0.91)]
        [TestCase("B0026127Y8", 0.8)]
        public async Task TestElectronics(string product, double accuracy)
        {
            log.LogInformation("TestElectronics: {0}", product);
            var testingClient = await Global.ElectronicBaseLine.Test(product, ProductCategory.Electronics).ConfigureAwait(false);
            Assert.GreaterOrEqual(Math.Round(testingClient.Performance.GetSingleAccuracy(true), 2), accuracy);
        }


        [TestCase("B0002L5R78", 0.8)]
        public async Task TestVideo(string product, double accuracy)
        {
            log.LogInformation("TestVideo: {0}", product);
            var testingClient = await Global.VideoBaseLine.Test(product, ProductCategory.Video).ConfigureAwait(false);
            Assert.GreaterOrEqual(Math.Round(testingClient.Performance.GetSingleAccuracy(true), 2), accuracy);
        }
    }
}
