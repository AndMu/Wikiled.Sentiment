using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NLog;
using NUnit.Framework;
using Wikiled.Redis.Config;
using Wikiled.Redis.Logic;
using Wikiled.Sentiment.Text.Cache;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Integration.Tests.Parser
{
    [TestFixture]
    public class RedisDocumentCacheTests
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private IRedisLink link;

        private RedisDocumentCache instance;

        private RedisInside.Redis redis;

        private LocalDocumentsCache local;

        private Mock<IMemoryCache> cache;

        [SetUp]
        public void Setup()
        {
            cache = new Mock<IMemoryCache>();
            local = new LocalDocumentsCache(new MemoryCache(new MemoryCacheOptions()));
            redis = new RedisInside.Redis(i => i.Port(6666).LogTo(item => log.Debug(item)));
            link = new RedisLink("Test", new RedisMultiplexer(new RedisConfiguration("localhost", 6666)));
            link.Open();
            instance = new RedisDocumentCache(POSTaggerType.Simple, link, local);
        }

        [TearDown]
        public void TearDown()
        {
            link.Close();
            redis.Dispose();
        }

        [Test]
        public async Task LoadSave()
        {
            Document document = new Document("Test");
            document.Id = Guid.NewGuid().ToString();
            document.Author = "Author";
            document.DocumentTime = new DateTime(2012, 02, 23);

            var result = await instance.GetCached(document).ConfigureAwait(false);
            Assert.IsNull(result);
            await instance.Save(document).ConfigureAwait(false);
            result = await instance.GetCached(document).ConfigureAwait(false);

            Assert.AreNotSame(document, result);
            Assert.AreEqual(document.Id, result.Id);
            Assert.AreEqual(document.Text, result.Text);
            Assert.AreEqual(document.Author, result.Author);

            result = await instance.GetCached(document).ConfigureAwait(false);
            document.Id = Guid.NewGuid().ToString();
            Assert.AreNotEqual(document.Id, result.Id);
            Assert.AreEqual(document.Text, result.Text);
            Assert.AreEqual(document.Author, result.Author);
        }
    }
}
