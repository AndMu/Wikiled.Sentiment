using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;
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

namespace Wikiled.Sentiment.Analysis.Containers
{
    public class SentimentServiceModule : IModule
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<SentimentServiceModule>();

        private readonly ConfigurationHandler configuration;

        public SentimentServiceModule(ConfigurationHandler configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public RedisConfiguration RedisConfiguration { get; set; }

        public string Name { get; set; }

        public string Lexicons { get; set; }

        public IServiceCollection ConfigureServices(IServiceCollection builder)
        {
            if (RedisConfiguration == null)
            {
                log.LogDebug("Using local cache");
                builder.AddScoped<ICachedDocumentsSource, LocalDocumentsCache>();
            }
            else
            {
                log.LogDebug("Using Redis cache");
                RedisConfiguration.ServiceName = Name;
                builder.RegisterModule(
                    new RedisModule(log, RedisConfiguration)
                    {
                        IsSingleInstance = false,
                        OpenOnConstruction = true
                    });

                builder.AddScoped<LocalDocumentsCache>();
                builder.AddScoped<ICacheFactory, RedisDocumentCacheFactory>();
            }

            if (!string.IsNullOrEmpty(Lexicons))
            {
                log.LogDebug("Adding lexicons");
                builder.AddSingleton<LexiconLoader>().AsSingleton<ILexiconLoader, LexiconLoader>(item => item.Load(Lexicons));
            }

            builder.AddSingleton<IConfigurationHandler>(configuration);
            builder.AddSingleton(new RecyclableConfig());
            builder.AddTransient<OpenNLPTextSplitter>();
            builder.AddTransient<Func<ITextSplitter>>(
                ctx => () => new RecyclableTextSplitter(ctx.GetService<ILogger<RecyclableTextSplitter>>(), ctx.GetService<OpenNLPTextSplitter>, new RecyclableConfig()),
                "underlying");

            return builder;
        }
    }
}
