using System.Configuration;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using Wikiled.Amazon.Logic;
using Wikiled.Redis.Config;
using Wikiled.Redis.Logic;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Splitters;
using Wikiled.Sentiment.Text.Cache;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.AcceptanceTests.Helpers
{
    public class TestHelper
    {
        public TestHelper(string server = "192.168.0.147", int port = 6373)
        {
            ConfigurationHandler configuration = new ConfigurationHandler();
            configuration.SetConfiguration("resources", Path.Combine(TestContext.CurrentContext.TestDirectory, ConfigurationManager.AppSettings["resources"]));
            Redis = new RedisLink("Wikiled", new RedisMultiplexer(new RedisConfiguration(server, port)));
            Redis.Open();
            AmazonRepository = new AmazonRepository(Redis);
            var cacheFactory = new RedisDocumentCacheFactory(Redis);
            Cache = cacheFactory.Create(POSTaggerType.SharpNLP);
            var localCache = new LocalCacheFactory(new MemoryCache(new MemoryCacheOptions()));
            SplitterHelper = new MainSplitterFactory(localCache, configuration).Create(POSTaggerType.SharpNLP);
        }

        public static TestHelper Instance { get; } = new TestHelper();

        public AmazonRepository AmazonRepository { get; }

        public ICachedDocumentsSource Cache { get; }

        public ISplitterHelper SplitterHelper { get; }

        public IRedisLink Redis { get; }
    }
}
