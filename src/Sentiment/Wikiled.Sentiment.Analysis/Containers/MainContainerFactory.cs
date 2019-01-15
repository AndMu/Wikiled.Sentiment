using Autofac;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Common.Extensions;
using Wikiled.Common.Logging;
using Wikiled.Common.Utilities.Modules;
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

        private readonly ContainerBuilder builder;

        private readonly Dictionary<string, bool> initialized = new Dictionary<string, bool>();

        private MainContainerFactory(ContainerBuilder builder)
        {
            this.builder = builder ?? throw new ArgumentNullException(nameof(builder));
            builder.RegisterModule<CommonModule>();
            builder.RegisterModule(new LoggingModule(ApplicationLogging.LoggerFactory));
            builder.RegisterModule(new SentimentMainModule());
            initialized["Splitter"] = false;
            initialized["Cache"] = false;
            initialized["Config"] = false;
        }

        public static MainContainerFactory CreateStandard()
        {
            log.LogInformation("CreateStandard");
            var instance = new MainContainerFactory(new ContainerBuilder());
            return instance.SetupLocalCache()
                .Config()
                .Splitter();
        }

        public static MainContainerFactory Setup(ContainerBuilder builder)
        {
            log.LogInformation("Setup");
            var instance = new MainContainerFactory(builder);
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
            var configuration = new ConfigurationHandler();
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
            Validate();

            IContainer container = builder.Build();
            var helper = new GlobalContainer(container);
            log.LogInformation("Initializing...");
            container.Resolve<IWordsHandler>();
            container.Resolve<ITextSplitter>();
            return helper;
        }

        public void Validate()
        {
            KeyValuePair<string, bool>[] notInitialized = initialized.Where(item => !item.Value).ToArray();
            if (notInitialized.Length > 0)
            {
                var modules = notInitialized.Select(item => item.Key).AccumulateItems(" ");
                throw new ApplicationException("Not all modules initialized: " + modules);
            }
        }
    }
}
