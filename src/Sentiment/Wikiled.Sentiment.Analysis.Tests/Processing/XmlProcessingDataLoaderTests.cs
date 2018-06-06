using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Processing;

namespace Wikiled.Sentiment.Analysis.Tests.Processing
{
    [TestFixture]
    public class XmlProcessingDataLoaderTests
    {
        [Test]
        public async Task LoadOldXml()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data/Antena.xml");
            var result = new XmlProcessingDataLoader().LoadOldXml(path);
            var positive = await result.All.Where(item => item.Sentiment == SentimentClass.Positive).Select(item => item.Data).ToArray();
            var negative = await result.All.Where(item => item.Sentiment == SentimentClass.Negative).Select(item => item.Data).ToArray();
            Assert.AreEqual(85, positive.Length);
            Assert.AreEqual(19, negative.Length);
            Assert.AreEqual(5.0, positive.First().Stars);
            Assert.AreEqual(120, positive.First().Text.Length);
        }
    }
}