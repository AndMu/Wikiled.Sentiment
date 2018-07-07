using System;
using NLog;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;

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

        public ISplitterHelper Create(POSTaggerType value)
        {
            log.Debug("Create: {0}", value);
            ISplitterHelper instance;
            switch (value)
            {
                case POSTaggerType.Simple:
                    instance = new SimpleSplitterHelper(configuration);
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
