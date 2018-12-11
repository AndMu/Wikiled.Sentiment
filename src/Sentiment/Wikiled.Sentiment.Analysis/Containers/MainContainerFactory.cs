using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Extensions;
using Wikiled.Common.Logging;
using Wikiled.Redis.Config;
using Wikiled.Redis.Logic;
using Wikiled.Sentiment.Text.Cache;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.NLP.OpenNLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Analysis.Containers
{
    public class MainContainerFactory
    {
         private static readonly ILogger log = ApplicationLogging.CreateLogger<MainContainerFactory>();

        private readonly ContainerBuilder builder = new ContainerBuilder();

        private readonly Dictionary<string, bool> initialized = new Dictionary<string, bool>();

        private MainContainerFactory()
        {
            builder.RegisterModule(new MainModule());
            initialized["Splitter"] = false;
            initialized["Cache"] = false;
            initialized["Config"] = false;
        }

        public static MainContainerFactory CreateStandard()
        {
            log.LogInformation("CreateStandard");
            MainContainerFactory instance = new MainContainerFactory();
            return instance.SetupLocalCache()
                .Config()
                .Splitter();
        }

        public static MainContainerFactory Setup()
        {
            log.LogInformation("Setup");
            MainContainerFactory instance = new MainContainerFactory();
            return instance;
        }

        public MainContainerFactory SetupLocalCache()
        {
            initialized["Cache"] = true;
            builder.RegisterType<LocalDocumentsCache>().As<ICachedDocumentsSource>();
            return this;
        }

        public MainContainerFactory SetupNullCache()
        {
            initialized["Cache"] = true;
            builder.RegisterType<NullCachedDocumentsSource>().As<ICachedDocumentsSource>();
            return this;
        }

        public MainContainerFactory SetupRedisCache(string name, string host, int port)
        {
            initialized["Cache"] = true;
            log.LogInformation("Using REDIS...");
            builder.Register(c => new RedisLink(name, new RedisMultiplexer(new RedisConfiguration(host, port)))).OnActivating(item => item.Instance.Open());
            builder.RegisterType<LocalDocumentsCache>();
            builder.RegisterType<RedisDocumentCacheFactory>().As<ICachedDocumentsSource>();
            return this;
        }

        public MainContainerFactory Config(Action<ConfigurationHandler> action = null)
        {
            initialized["Config"] = true;
            ConfigurationHandler configuration = new ConfigurationHandler();
            action?.Invoke(configuration);
            builder.RegisterInstance(configuration).As<IConfigurationHandler>();
            return this;
        }

        public MainContainerFactory Splitter(POSTaggerType value = POSTaggerType.SharpNLP)
        {
            initialized["Splitter"] = true;
            switch (value)
            {
                case POSTaggerType.Simple:
                    builder.RegisterType<SimpleTextSplitter>().Named<ITextSplitter>("Underlying");
                    break;
                case POSTaggerType.SharpNLP:
                    builder.Register(
                            c => new RecyclableTextSplitter(c.ResolveNamed<Func<ITextSplitter>>("UnderlyingNested"), 5000))
                        .Named<ITextSplitter>("Underlying");
                    builder.RegisterType<OpenNLPTextSplitter>().Named<ITextSplitter>("UnderlyingNested");
                    break;
                default:
                    throw new NotSupportedException(value.ToString());
            }

            builder.Register(c => value);
            return this;
        }

        public IGlobalContainer Create()
        {
            var notInitialized = initialized.Where(item => !item.Value).ToArray();
            if (notInitialized.Length > 0)
            {
                var modules = notInitialized.Select(item => item.Key).AccumulateItems(" ");
                throw new ApplicationException("Not all modules initialized: " + modules);
            }

            var container = builder.Build();
            var helper = new GlobalContainer(container);
            log.LogInformation("Initializing...");
            container.Resolve<IWordsHandler>();
            container.Resolve<ITextSplitter>();
            return helper;
        }
    }
}
