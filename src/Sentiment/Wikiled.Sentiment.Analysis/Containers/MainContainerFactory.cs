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
using Wikiled.Sentiment.Text.Config;
using Wikiled.Sentiment.Text.NLP.NER;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Analysis.Containers
{
    public class MainContainerFactory
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<MainContainerFactory>();

        private readonly IServiceCollection builder;

        private readonly Dictionary<string, bool> initialized = new Dictionary<string, bool>();

        private string libraryPath;

        private MainContainerFactory(IServiceCollection builder)
        {
            this.builder = builder ?? throw new ArgumentNullException(nameof(builder));
            builder.RegisterModule<CommonModule>();
            builder.RegisterModule(new LoggingModule(ApplicationLogging.LoggerFactory));
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

        public MainContainerFactory Config(string path = null)
        {
            initialized["Config"] = true;
            libraryPath = path;
            return this;
        }

        public MainContainerFactory AddNER(INamedEntityRecognition nerRecognition)
        {
            builder.AddSingleton(nerRecognition);
            return this;
        }

        public MainContainerFactory Splitter(POSTaggerType value = POSTaggerType.SharpNLP, bool useNER = false)
        {
            initialized["Splitter"] = true;
            builder.RegisterModule(
                new SentimentMainModule
                {
                    Tagger = value,
                    LibraryPath = libraryPath,
                    UseNER = useNER
                });

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
