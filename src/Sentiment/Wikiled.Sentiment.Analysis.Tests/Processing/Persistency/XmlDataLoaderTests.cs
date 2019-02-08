using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Persistency;

namespace Wikiled.Sentiment.Analysis.Tests.Processing.Persistency
{
    [TestFixture]
    public class XmlDataLoaderTests
    {
        [Test]
        public async Task LoadOldXml()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data/Antena.xml");
            var result = new XmlDataLoader(new NullLogger<XmlDataLoader>()).LoadOldXml(path);
            var positive = await result.Load().Where(item => item.Sentiment == SentimentClass.Positive).Select(item => item.Data).ToArray();
            var negative = await result.Load().Where(item => item.Sentiment == SentimentClass.Negative).Select(item => item.Data).ToArray();
            Assert.AreEqual(85, positive.Length);
            Assert.AreEqual(19, negative.Length);
            Assert.AreEqual(5.0, positive.First().Result.Stars);
            Assert.AreEqual(120, positive.First().Result.Text.Length);
        }
    }
}