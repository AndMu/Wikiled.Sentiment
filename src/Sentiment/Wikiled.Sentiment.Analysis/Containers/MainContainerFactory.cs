using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using NLog;
using Wikiled.Common.Extensions;
using Wikiled.Redis.Config;
using Wikiled.Redis.Logic;
using Wikiled.Sentiment.Text.Cache;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.NLP.OpenNLP;
using Wikiled.Sentiment.Text.NLP.Repair;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Analysis.Containers
{
    public class MainContainerFactory
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly ContainerBuilder builder = new ContainerBuilder();

        private Dictionary<string, bool> initialized = new Dictionary<string, bool>();

        private MainContainerFactory()
        {
            builder.RegisterModule(new MainModule());
            initialized["Repair"] = false;
            initialized["Splitter"] = false;
            initialized["Cache"] = false;
            initialized["Config"] = false;
            initialized["Context"] = false;
        }

        public static MainContainerFactory CreateStandard()
        {
            log.Info("CreateStandard");
            MainContainerFactory instance = new MainContainerFactory();
            return instance.SetupRepair()
                .SetupLocalCache()
                .WithContext()
                .Config()
                .Splitter();
        }

        public static MainContainerFactory Setup()
        {
            log.Info("Setup");
            MainContainerFactory instance = new MainContainerFactory();
            return instance;
        }

        public MainContainerFactory SetupRepair(bool supportRepair = true)
        {
            initialized["Repair"] = true;
            if (supportRepair)
            {
                builder.RegisterType<SentenceRepairHandler>().As<ISentenceRepairHandler>().SingleInstance();
            }
            else
            {
                builder.RegisterType<NullSentenceRepairHandler>().As<ISentenceRepairHandler>().SingleInstance();
                // add as specific if somebody still wants it
                builder.RegisterType<SentenceRepairHandler>().SingleInstance();
            }

            return this;
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
            builder.RegisterInstance(NullCachedDocumentsSource.Instance).As<ICachedDocumentsSource>();
            return this;
        }

        public MainContainerFactory SetupRedisCache(string name, string host, int port)
        {
            initialized["Cache"] = true;
            log.Info("Using REDIS...");
            builder.Register(c => new RedisLink(name, new RedisMultiplexer(new RedisConfiguration(host, port)))).OnActivating(item => item.Instance.Open());
            builder.RegisterType<RedisDocumentCacheFactory>().As<ICachedDocumentsSource>();
            return this;
        }

        public MainContainerFactory WithContext(Action<SentimentContext> action = null)
        {
            initialized["Context"] = true;
            SentimentContext context = new SentimentContext();
            action?.Invoke(context);
            builder.RegisterInstance(context).As<ISentimentContext>();
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

        public IContainerHelper Create()
        {
            var notInitialized = initialized.Where(item => !item.Value).ToArray();
            if (notInitialized.Length > 0)
            {
                var modules = notInitialized.Select(item => item.Key).AccumulateItems(" ");
                throw new ApplicationException("Not all modules initialized: " + modules);
            }

            var helper = new ContainerHelper(builder.Build());
            log.Info("Initializing...");
            helper.Container.Resolve<IWordsHandler>();
            helper.Container.Resolve<ITextSplitter>();
            return helper;
        }
    }
}
