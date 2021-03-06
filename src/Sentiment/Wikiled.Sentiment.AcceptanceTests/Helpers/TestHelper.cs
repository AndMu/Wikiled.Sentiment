﻿using Microsoft.Extensions.DependencyInjection;
using Wikiled.Amazon.Logic;
using Wikiled.Common.Utilities.Modules;
using Wikiled.Redis.Config;
using Wikiled.Sentiment.Analysis.Containers;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;

namespace Wikiled.Sentiment.AcceptanceTests.Helpers
{
    public class TestHelper
    {
        private readonly IGlobalContainer container;

        public TestHelper(string server = "192.168.0.70", int port = 6373)
        {
            IServiceCollection service = new ServiceCollection();
            service.RegisterModule(new RedisServerModule(new RedisConfiguration(server, port) {ServiceName = "Wikiled"}));
            service.AddSingleton<AmazonRepository>();

            container = MainContainerFactory
                        .Setup(service)
                        .SetupLocalCache()
                        .Config()
                        .Splitter()
                        .Create();

            Reset();
        }

        public static TestHelper Instance { get; } = new TestHelper();

        public void Reset()
        {
            ContainerHelper?.Dispose();
            ContainerHelper = container.StartSession();
        }

        public AmazonRepository AmazonRepository => ContainerHelper.Resolve<AmazonRepository>();

        public ISessionContainer ContainerHelper { get; private set; }

    }
}
