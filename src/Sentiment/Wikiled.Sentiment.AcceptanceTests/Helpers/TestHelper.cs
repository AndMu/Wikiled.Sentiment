using System;
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
        private readonly Lazy<RedisLink> redis;

        private readonly Lazy<AmazonRepository> amazonRepository;

        private readonly Lazy<ICachedDocumentsSource> cachedDocumentSource;

        public TestHelper(string server = "192.168.0.147", int port = 6373)
        {
            ConfigurationHandler configuration = new ConfigurationHandler();
            configuration.SetConfiguration("resources", Path.Combine(TestContext.CurrentContext.TestDirectory, ConfigurationManager.AppSettings["resources"]));
            redis = new Lazy<RedisLink>(() =>
            {
                var instance = new RedisLink("Wikiled", new RedisMultiplexer(new RedisConfiguration(server, port)));
                instance.Open();
                return instance;
            });

            amazonRepository = new Lazy<AmazonRepository>(() => new AmazonRepository(Redis));
            cachedDocumentSource = new Lazy<ICachedDocumentsSource>(() =>
            {
                var cacheFactory = new RedisDocumentCacheFactory(Redis);
                return cacheFactory.Create(POSTaggerType.SharpNLP);
            });

            var localCache = new LocalCacheFactory(new MemoryCache(new MemoryCacheOptions()));
            SplitterHelper = new MainSplitterFactory(localCache, configuration).Create(POSTaggerType.SharpNLP);
        }

        public static TestHelper Instance { get; } = new TestHelper();

        public AmazonRepository AmazonRepository => amazonRepository.Value;

        public ICachedDocumentsSource Cache => cachedDocumentSource.Value;

        public ISplitterHelper SplitterHelper { get; }

        public IRedisLink Redis => redis.Value;
    }
}
