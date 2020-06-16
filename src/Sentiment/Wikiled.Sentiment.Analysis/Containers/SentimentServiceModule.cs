using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Wikiled.Common.Logging;
using Wikiled.Common.Utilities.Modules;
using Wikiled.Redis.Config;
using Wikiled.Redis.Modules;
using Wikiled.Sentiment.Text.Cache;
using Wikiled.Sentiment.Text.Config;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.NLP.OpenNLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Cache;

namespace Wikiled.Sentiment.Analysis.Containers
{
    public class SentimentServiceModule : IModule
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<SentimentServiceModule>();

        public RedisConfiguration RedisConfiguration { get; set; }

        public string Name { get; set; }

        public IServiceCollection ConfigureServices(IServiceCollection builder)
        {
            if (RedisConfiguration == null)
            {
                log.LogDebug("Using local cache");
                builder.AddSingleton<ICachedDocumentsSource, LocalDocumentsCache>();
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

                builder.AddSingleton<LocalDocumentsCache>();
                builder.AddSingleton<ICacheFactory, RedisDocumentCacheFactory>();
            }

            builder.AddSingleton<LexiconLoader>().As<ILexiconLoader, LexiconLoader>();
            builder.AddSingleton(new RecyclableConfig());
            builder.AddTransient<OpenNLPTextSplitter>();

            builder.AddTransient<Func<ITextSplitter>>(
                ctx => () => new RecyclableTextSplitter(
                    ctx.GetService<ILogger<RecyclableTextSplitter>>(),
                    ctx.GetService<OpenNLPTextSplitter>,
                    new RecyclableConfig()),
                "underlying");

            return builder;
        }
    }
}
