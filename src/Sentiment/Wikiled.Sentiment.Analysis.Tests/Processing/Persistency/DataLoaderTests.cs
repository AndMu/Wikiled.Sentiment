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
    public class DataLoaderTests
    {
        private DataLoader instance;

        private SimpleDataSource source;

        [SetUp]
        public void SetUp()
        {
            source = new SimpleDataSource();
            instance = CreateDataLoader();
        }

        [Test]
        public async Task LoadJson()
        {
            source.All = Path.Combine(TestContext.CurrentContext.TestDirectory, @"data\data.json");
            var all = await instance.Load(source).Load().ToArray();
            Assert.AreEqual(3, all.Length);
            Assert.AreEqual(1, all.Count(item => item.Sentiment == SentimentClass.Negative));
            Assert.AreEqual(2, all.Count(item => item.Sentiment == SentimentClass.Positive));
        }

        private DataLoader CreateDataLoader()
        {
            return new DataLoader(new NullLoggerFactory());
        }
    }
}