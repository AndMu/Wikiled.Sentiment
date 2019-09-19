using System;
using System.Configuration;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Wikiled.Amazon.Logic;
using Wikiled.Redis.Config;
using Wikiled.Redis.Logic;
using Wikiled.Sentiment.Analysis.Containers;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;

namespace Wikiled.Sentiment.AcceptanceTests.Helpers
{
    public class TestHelper
    {
        private readonly Lazy<IRedisLink> redis;

        private readonly Lazy<AmazonRepository> amazonRepository;

        private readonly IGlobalContainer container;

        public TestHelper(string server = "192.168.0.70", int port = 6373)
        {
            redis = new Lazy<IRedisLink>(() =>
            {
                var serverInstance = new RedisServer(new RedisConfiguration(server, port) { ServiceName = "Wikiled" });
                return serverInstance.Provider.GetService<IRedisLink>();
            });

            amazonRepository = new Lazy<AmazonRepository>(() => new AmazonRepository(Redis));
            container = MainContainerFactory
                              .Setup(new ServiceCollection())
                              .SetupLocalCache()
                              .Config(item => item.SetConfiguration("resources", Path.Combine(TestContext.CurrentContext.TestDirectory, ConfigurationManager.AppSettings["resources"])))
                              .Splitter()
                              .Create();

            Reset();
        }

        public static TestHelper Instance { get; } = new TestHelper();

        public void Reset()
        {
            ContainerHelper = container.StartSession();
        }

        public AmazonRepository AmazonRepository => amazonRepository.Value;

        public ISessionContainer ContainerHelper { get; private set; }

        public IRedisLink Redis => redis.Value;
    }
}
