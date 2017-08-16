using System;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Resources;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Analysis.Processing.Splitters
{
    public class SplitterFactory
    {
        private readonly ICacheFactory cacheFactory;

        private readonly ConfigurationHandler configuration;

        public SplitterFactory(ICacheFactory cacheFactory, ConfigurationHandler configuration)
        {
            Guard.NotNull(() => cacheFactory, cacheFactory);
            Guard.NotNull(() => configuration, configuration);
            this.configuration = configuration;
            this.cacheFactory = cacheFactory;
        }

        public ISplitterHelper Create(POSTaggerType value)
        {
            switch (value)
            {
                case POSTaggerType.Simple:
                    return new SimpleSplitterHelper(configuration);
                case POSTaggerType.Stanford:
                    return new StanfordSplitterHelper(cacheFactory, configuration);
                case POSTaggerType.SharpNLP:
                    return new OpenNlpSplitterHelper(cacheFactory, configuration);
                default:
                    throw new NotSupportedException(value.ToString());
            }
        }
    }
}
