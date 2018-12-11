using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Wikiled.Amazon.Logic;
using Wikiled.Common.Logging;

namespace Wikiled.Sentiment.AcceptanceTests.Training
{
    [TestFixture]
    public class CrossDomainTests
    {
         private static readonly ILogger log = ApplicationLogging.CreateLogger<CrossDomainTests>();

        [TestCase("B0026127Y8", ProductCategory.Video, 9)]
        [TestCase("9562910334", ProductCategory.Book, 9)]
        public async Task TestElectronics(string product, ProductCategory category, int accuracy)
        {
            log.LogInformation("TestElectronics: {0} {1}", product, category);
            var result = await Global.ElectronicBaseLine.Test(product, category).ConfigureAwait(false);
            Assert.GreaterOrEqual(Math.Ceiling(result.Performance.GetSingleAccuracy(true) * 10), accuracy);
        }

        [TestCase("B0002L5R78", ProductCategory.Electronics, 9)]
        public async Task TestVideo(string product, ProductCategory category, int accuracy)
        {
            log.LogInformation("TestVideo: {0} {1}", product, category);
            var result = await Global.VideoBaseLine.Test(product, category).ConfigureAwait(false);
            Assert.GreaterOrEqual(Math.Ceiling(result.Performance.GetSingleAccuracy(true) * 10), accuracy);
        }
    }
}
