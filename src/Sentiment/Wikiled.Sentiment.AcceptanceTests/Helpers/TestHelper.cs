using System;
using System.Configuration;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using Wikiled.Amazon.Logic;
using Wikiled.Redis.Config;
using Wikiled.Redis.Logic;
using Wikiled.Sentiment.Analysis.Processing.Containers;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.AcceptanceTests.Helpers
{
    public class TestHelper
    {
        private readonly Lazy<RedisLink> redis;

        private readonly Lazy<AmazonRepository> amazonRepository;

        public TestHelper(string server = "192.168.0.70", int port = 6373)
        {
            redis = new Lazy<RedisLink>(() =>
            {
                var instance = new RedisLink("Wikiled", new RedisMultiplexer(new RedisConfiguration(server, port)));
                instance.Open();
                return instance;
            });

            amazonRepository = new Lazy<AmazonRepository>(() => new AmazonRepository(Redis));
            ContainerHelper = MainContainerFactory
                .Setup()
                .SetupRepair()
                .SetupLocalCache()
                .WithContext()
                .Config(item => item.SetConfiguration("resources", Path.Combine(TestContext.CurrentContext.TestDirectory, ConfigurationManager.AppSettings["resources"])))
                .Splitter()
                .Create();
        }

        public static TestHelper Instance { get; } = new TestHelper();

        public AmazonRepository AmazonRepository => amazonRepository.Value;

        public IContainerHelper ContainerHelper { get; }

        public IRedisLink Redis => redis.Value;
    }
}
