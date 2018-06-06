using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Processing;

namespace Wikiled.Sentiment.Analysis.Tests.Processing
{
    [TestFixture]
    public class DataLoaderTests
    {
        private DataLoader instance;

        [SetUp]
        public void SetUp()
        {
            instance = CreateDataLoader();
        }

        [Test]
        public async Task Load()
        {
            var data = instance.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, @"data\data.json"));
            var all = await data.All.ToArray();
            Assert.AreEqual(3, all.Length);
            Assert.AreEqual(1, all.Count(item => item.Sentiment == SentimentClass.Negative));
            Assert.AreEqual(2, all.Count(item => item.Sentiment == SentimentClass.Positive));
        }

        private DataLoader CreateDataLoader()
        {
            return new DataLoader();
        }
    }
}