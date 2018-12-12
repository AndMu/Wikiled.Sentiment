using Autofac;
using Microsoft.Extensions.Logging;
using System;
using Wikiled.Common.Logging;
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
    public class SentimentServiceModule : Module
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

        protected override void Load(ContainerBuilder builder)
        {
            if (RedisConfiguration == null)
            {
                log.LogDebug("Using local cache");
                builder.RegisterType<LocalDocumentsCache>().As<ICachedDocumentsSource>();
            }
            else
            {
                log.LogDebug("Using Redis cache");
                builder.RegisterModule(new RedisModule(Name, RedisConfiguration));
                builder.RegisterType<LocalDocumentsCache>();
                builder.RegisterType<RedisDocumentCacheFactory>().As<ICachedDocumentsSource>();
            }

            if (!string.IsNullOrEmpty(Lexicons))
            {
                log.LogDebug("Adding lexicons");
                builder.RegisterType<LexiconLoader>().As<ILexiconLoader>().OnActivating(item => item.Instance.Load(Lexicons));
            }

            builder.RegisterInstance(configuration).As<IConfigurationHandler>();
            builder.Register(
                    c => new RecyclableTextSplitter(c.ResolveNamed<Func<ITextSplitter>>("UnderlyingNested"), 5000))
                .Named<ITextSplitter>("Underlying");
            builder.RegisterType<OpenNLPTextSplitter>().Named<ITextSplitter>("UnderlyingNested");
        }
    }
}
