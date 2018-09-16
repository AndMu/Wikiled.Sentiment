using Autofac;
using NLog;
using System;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.NLP.Repair;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.Dictionary;
using Wikiled.Text.Analysis.NLP.NRC;
using Wikiled.Text.Analysis.POS;
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

        public IContainerHelper Create(POSTaggerType value)
        {
            log.Debug("Create: {0}", value);
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterInstance(cacheFactory);
            builder.RegisterInstance(configuration);
            builder.RegisterType<LexiconConfiguration>().As<ILexiconConfiguration>().SingleInstance();
            builder.RegisterType<BasicEnglishDictionary>().As<IWordsDictionary>().SingleInstance();
            builder.RegisterType<InquirerManager>().As<IInquirerManager>().SingleInstance().OnActivated(item => item.Instance.Load());
            builder.RegisterType<NRCDictionary>().As<INRCDictionary>().SingleInstance().OnActivated(item => item.Instance.Load());
            builder.RegisterType<WordOccurenceFactory>().As<IWordFactory>().SingleInstance();
            if (SupportRepair)
            {
                builder.RegisterType<SentenceRepairHandler>().As<ISentenceRepairHandler>().SingleInstance();
            }
            else
            {
                builder.Register(item => (ISentenceRepairHandler)null).As<ISentenceRepairHandler>();
            }

            builder.RegisterType<WordsDataLoader>().As<IWordsHandler>().SingleInstance();
            builder.RegisterType<AspectSerializer>().As<IWordsHandler>().SingleInstance();
            builder.RegisterType<QueueTextSplitter>().As<ITextSplitter>();

            switch (value)
            {
                case POSTaggerType.Simple:
                    //builder.Register(c => new SimpleTextSplitter(c.Resolve<ISentenceTokenizerFactory>()));
                    //builder.Register(c => new SimpleTextSplitter(c.Resolve<ISentenceTokenizerFactory>()));
                    break;
                case POSTaggerType.SharpNLP:
                    //builder.Register(c => new RecyclableTextSplitter(c.Resolve<ISentenceTokenizerFactory>()));
                    //builder.RegisterType<RecyclableTextSplitter>().As<ITextSplitter>();
                    break;
                default:
                    throw new NotSupportedException(value.ToString());
            }

            return new ContainerHelper(builder.Build());
        }
    }
}
