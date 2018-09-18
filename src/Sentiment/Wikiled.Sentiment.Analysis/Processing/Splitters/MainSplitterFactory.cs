using Autofac;
using Microsoft.Extensions.Caching.Memory;
using NLog;
using System;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.NLP.OpenNLP;
using Wikiled.Sentiment.Text.NLP.Repair;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.Dictionary;
using Wikiled.Text.Analysis.NLP;
using Wikiled.Text.Analysis.NLP.Frequency;
using Wikiled.Text.Analysis.NLP.NRC;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.Tokenizer.Pipelined;
using Wikiled.Text.Analysis.Words;
using Wikiled.Text.Inquirer.Logic;

namespace Wikiled.Sentiment.Analysis.Processing.Splitters
{
    public class MainSplitterFactory : IMainSplitterFactory
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly ICacheFactory cacheFactory;

        private readonly IConfigurationHandler configuration;

        public MainSplitterFactory(ICacheFactory cacheFactory, IConfigurationHandler configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.cacheFactory = cacheFactory ?? throw new ArgumentNullException(nameof(cacheFactory));
        }

        public bool SupportRepair { get; set; } = true;

        public IContainerHelper Create(POSTaggerType value, SentimentContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            log.Debug("Create: {0}", value);
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterInstance(cacheFactory);
            builder.Register(c => cacheFactory.Create(value)).SingleInstance();
            builder.RegisterInstance(configuration);
            builder.RegisterInstance(context).As<ISentimentContext>();
            builder.RegisterType<LexiconConfiguration>().As<ILexiconConfiguration>().SingleInstance();
            builder.RegisterType<BasicEnglishDictionary>().As<IWordsDictionary>().SingleInstance();
            builder.RegisterType<InquirerManager>().As<IInquirerManager>().SingleInstance().OnActivating(item => item.Instance.Load());
            builder.RegisterType<NRCDictionary>().As<INRCDictionary>().SingleInstance().OnActivating(item => item.Instance.Load());
            builder.RegisterType<ParsedReviewManager>().As<IParsedReviewManager>();
            
            builder.RegisterType<SentenceTokenizerFactory>().As<ISentenceTokenizerFactory>().SingleInstance();
            builder.RegisterType<NaivePOSTagger>().As<IPOSTagger>().SingleInstance();
            builder.RegisterType<BNCList>().As<IPosTagResolver>().As<IWordFrequencyList>().SingleInstance();
            builder.Register(c => WordTypeResolver.Instance).As<IWordTypeResolver>().SingleInstance();

            builder.RegisterType<RawWordExtractor>().As<IRawTextExtractor>().SingleInstance();
            builder.Register(c => new MemoryCache(new MemoryCacheOptions())).As<IMemoryCache>().SingleInstance();

            builder.RegisterType<WordOccurenceFactory>().As<IWordFactory>();
            if (SupportRepair)
            {
                builder.RegisterType<SentenceRepairHandler>().As<ISentenceRepairHandler>().SingleInstance();
            }
            else
            {
                builder.RegisterType<NullSentenceRepairHandler>().As<ISentenceRepairHandler>().SingleInstance();
                // add as specific if somebody still wants it
                builder.RegisterType<SentenceRepairHandler>().SingleInstance();
            }

            int parallel = Environment.ProcessorCount / 2;
            builder.RegisterType<WordsDataLoader>().As<IWordsHandler>().SingleInstance().OnActivating(item => item.Instance.Load());
            builder.RegisterType<AspectSerializer>().As<IAspectSerializer>().SingleInstance();
            builder.Register(item => new QueueTextSplitter(parallel, item.ResolveNamed<Func<ITextSplitter>>("Underlying"))).As<ITextSplitter>().SingleInstance();

            switch (value)
            {
                case POSTaggerType.Simple:
                    builder.RegisterType<SimpleTextSplitter>().Named<ITextSplitter>("Underlying");
                    break;
                case POSTaggerType.SharpNLP:
                    builder.Register(c => new RecyclableTextSplitter(c.ResolveNamed<Func<ITextSplitter>>("UnderlyingNested"), 5000)).Named<ITextSplitter>("Underlying");
                    builder.RegisterType<OpenNLPTextSplitter>().Named<ITextSplitter>("UnderlyingNested");
                    break;
                default:
                    throw new NotSupportedException(value.ToString());
            }

            var helper = new ContainerHelper(builder.Build(), context);
            log.Info("Initializing...");
            helper.GetDataLoader();
            helper.GetTextSplitter();
            return helper;
        }
    }
}
