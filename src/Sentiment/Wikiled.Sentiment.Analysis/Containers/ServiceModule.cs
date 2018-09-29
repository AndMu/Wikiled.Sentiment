using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using NLog;
using Wikiled.Redis.Config;
using Wikiled.Redis.Logic;
using Wikiled.Sentiment.Text.Cache;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.NLP.OpenNLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Text.Analysis.Cache;

namespace Wikiled.Sentiment.Analysis.Containers
{
    public class ServiceModule : Module
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly ConfigurationHandler configuration;

        public ServiceModule(ConfigurationHandler configuration)
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
                log.Debug("Using local cache");
                builder.RegisterType<LocalDocumentsCache>().As<ICachedDocumentsSource>();
            }
            else
            {
                log.Debug("Using Redis cache");
                builder.Register(c => new RedisLink(Name, new RedisMultiplexer(RedisConfiguration))).OnActivating(item => item.Instance.Open());
                builder.RegisterType<RedisDocumentCacheFactory>().As<ICachedDocumentsSource>();
            }

            if (!string.IsNullOrEmpty(Lexicons))
            {
                log.Debug("Adding lexicons");
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
