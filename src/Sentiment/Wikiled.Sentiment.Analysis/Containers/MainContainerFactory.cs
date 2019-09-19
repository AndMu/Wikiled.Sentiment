using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Common.Extensions;
using Wikiled.Common.Logging;
using Wikiled.Common.Utilities.Modules;
using Wikiled.Redis.Config;
using Wikiled.Redis.Modules;
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

        private readonly IServiceCollection builder;

        private readonly Dictionary<string, bool> initialized = new Dictionary<string, bool>();

        private MainContainerFactory(IServiceCollection builder)
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
            var instance = new MainContainerFactory(new ServiceCollection());
            return instance.SetupLocalCache()
                .Config()
                .Splitter();
        }

        public static MainContainerFactory Setup(IServiceCollection builder)
        {
            log.LogInformation("Setup");
            var instance = new MainContainerFactory(builder);
            return instance;
        }

        public MainContainerFactory SetupLocalCache()
        {
            initialized["Cache"] = true;
            builder.AddScoped<ICachedDocumentsSource, LocalDocumentsCache>();
            return this;
        }

        public MainContainerFactory SetupNullCache()
        {
            initialized["Cache"] = true;
            builder.AddScoped<ICachedDocumentsSource, NullCachedDocumentsSource>();
            return this;
        }

        public MainContainerFactory SetupRedisCache(string name, string host, int port)
        {
            initialized["Cache"] = true;
            log.LogInformation("Using REDIS...");
            builder.RegisterModule(
                new RedisModule(log, new RedisConfiguration(host, port))
                {
                    IsSingleInstance = false,
                    OpenOnConstruction = true
                });
            builder.AddScoped<ICacheFactory, RedisDocumentCacheFactory>();
            builder.AddScoped<LocalDocumentsCache>().As<ICachedDocumentsSource, LocalDocumentsCache>();
            return this;
        }

        public MainContainerFactory Config(Action<ConfigurationHandler> action = null)
        {
            initialized["Config"] = true;
            var configuration = new ConfigurationHandler();
            action?.Invoke(configuration);
            builder.AddSingleton<IConfigurationHandler>(configuration);
            return this;
        }

        public MainContainerFactory Splitter(POSTaggerType value = POSTaggerType.SharpNLP)
        {
            initialized["Splitter"] = true;
            switch (value)
            {
                case POSTaggerType.Simple:
                    builder.AddTransient<ITextSplitter, SimpleTextSplitter>();
                    break;
                case POSTaggerType.SharpNLP:
                    builder.AddTransient<OpenNLPTextSplitter>();
                    builder.AddTransient<Func<ITextSplitter>>(
                        ctx => () => new RecyclableTextSplitter(ctx.GetService<ILogger<RecyclableTextSplitter>>(), ctx.GetService<OpenNLPTextSplitter>, new RecyclableConfig()),
                        "underlying");
                    break;
                default:
                    throw new NotSupportedException(value.ToString());
            }

            return this;
        }

        public IGlobalContainer Create()
        {
            Validate();

            var container = builder.BuildServiceProvider();
            var helper = new GlobalContainer(container);
            log.LogInformation("Initializing...");
            container.GetService<IWordsHandler>();
            container.GetService<ITextSplitter>();
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
