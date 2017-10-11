using System;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Resources;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Analysis.Processing.Splitters
{
    public class MainSplitterFactory : IMainSplitterFactory
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly ICacheFactory cacheFactory;

        private readonly ConfigurationHandler configuration;

        public MainSplitterFactory(ICacheFactory cacheFactory, ConfigurationHandler configuration)
        {
            Guard.NotNull(() => cacheFactory, cacheFactory);
            Guard.NotNull(() => configuration, configuration);
            this.configuration = configuration;
            this.cacheFactory = cacheFactory;
        }

        public ISplitterHelper Create(POSTaggerType value)
        {
            log.Debug("Create: {0}", value);
            ISplitterHelper instance;
            switch (value)
            {
                case POSTaggerType.Simple:
                    instance = new SimpleSplitterHelper(configuration);
                    break;
                case POSTaggerType.Stanford:
                    instance = new StanfordSplitterHelper(cacheFactory, configuration);
                    break;
                case POSTaggerType.SharpNLP:
                    instance = new OpenNlpSplitterHelper(cacheFactory, configuration);
                    break;
                default:
                    throw new NotSupportedException(value.ToString());
            }

            log.Debug("Loading lexicon...");
            instance.Load();
            return instance;
        }
    }
}
