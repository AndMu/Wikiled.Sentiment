using System.IO;
using NUnit.Framework;
using Wikiled.Core.Utility.Resources;
using Wikiled.Redis.Config;
using Wikiled.Redis.Logic;
using Wikiled.Sentiment.Analysis.Amazon;
using Wikiled.Sentiment.Analysis.Amazon.Logic;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Splitters;
using Wikiled.Sentiment.Text.Cache;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.AcceptanceTests.Helpers
{
    public class TestHelper
    {
        public TestHelper(string server = "gitlab", int port = 6373)
        {
            ConfigurationHandler configuration = new ConfigurationHandler();
            configuration.SetConfiguration("resources", Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\..\Resources"));
            configuration.SetConfiguration("Stanford", Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\..\Resources\Stanford"));
            Redis = new RedisLink("Wikiled", new RedisMultiplexer(new RedisConfiguration(server, port)));
            Redis.Open();
            AmazonRepository = new AmazonRepository(Redis);
            var cacheFactory = new RedisDocumentCacheFactory(Redis);
            Cache = cacheFactory.Create(POSTaggerType.Stanford);
            CachedSplitterHelper = new SplitterFactory(cacheFactory, configuration).Create(POSTaggerType.Stanford);
            var localCache = new LocalCacheFactory();
            NonCachedSplitterHelper = new SplitterFactory(localCache, configuration).Create(POSTaggerType.Stanford);
        }

        public ICachedDocumentsSource Cache { get; }

        public ISplitterHelper CachedSplitterHelper { get; }

        public ISplitterHelper NonCachedSplitterHelper { get; }

        public IRedisLink Redis { get; }

        public AmazonRepository AmazonRepository { get; }

        public static TestHelper Instance { get; } = new TestHelper();
    }
}
