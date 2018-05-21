using System.IO;
using System.Linq;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Processing;

namespace Wikiled.Sentiment.Analysis.Tests.Processing
{
    [TestFixture]
    public class XmlProcessingDataLoaderTests
    {
        [Test]
        public void LoadOldXml()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data/Antena.xml");
            var result = new XmlProcessingDataLoader().LoadOldXml(path);
            Assert.AreEqual(85, result.Positive.Count());
            Assert.AreEqual(19, result.Negative.Count());
            Assert.AreEqual(5.0, result.Positive.First().Stars);
            Assert.AreEqual(120, result.Positive.First().Text.Length);
        }
    }
}