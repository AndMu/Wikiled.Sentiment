using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Redis.Config;
using Wikiled.Redis.Logic;
using Wikiled.Sentiment.Text.Cache;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Integration.Tests.Parser
{
    [TestFixture]
    public class RedisDocumentCacheTests
    {
        private IRedisLink link;

        private RedisDocumentCache instance;

        [SetUp]
        public void Setup()
        {
            link = new RedisLink("Test", new RedisMultiplexer(new RedisConfiguration("localhost", 6666)));
            link.Open();
            instance = new RedisDocumentCache(POSTaggerType.Simple, link);
        }

        [TearDown]
        public void TearDown()
        {
            link.Close();
        }

        [Test]
        public async Task LoadSave()
        {
            Document document = new Document("Test");
            document.Id = Guid.NewGuid().ToString();
            document.Author = "Author";
            document.DocumentTime = new DateTime(2012, 02, 23);

            var result = await instance.GetById(document.Id).ConfigureAwait(false);
            Assert.IsNull(result);
            await instance.Save(document).ConfigureAwait(false);
            result = await instance.GetById(document.Id).ConfigureAwait(false);
            // Maybe that is an issue - we need clonning? Assert.AreNotSame(document, result);
            Assert.AreEqual(document.Id, result.Id);
            Assert.AreEqual(document.Text, result.Text);
            Assert.AreEqual(document.Author, result.Author);
        }
    }
}
