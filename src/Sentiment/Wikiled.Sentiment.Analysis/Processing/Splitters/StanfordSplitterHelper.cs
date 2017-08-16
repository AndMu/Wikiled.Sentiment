using System.IO;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Resources;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.NLP.Stanford;
using Wikiled.Text.Analysis.Cache;

namespace Wikiled.Sentiment.Analysis.Processing.Splitters
{
    public class StanfordSplitterHelper : BaseSplitterHelper
    {
        private readonly ICacheFactory cacheFactory;

        private readonly ConfigurationHandler configuration;

        public StanfordSplitterHelper(ICacheFactory cacheFactory, ConfigurationHandler configuration, int parallel = 0)
            : base(configuration, parallel)
        {
            Guard.NotNull(() => cacheFactory, cacheFactory);
            Guard.NotNull(() => configuration, configuration);
            this.configuration = configuration;
            this.cacheFactory = cacheFactory;
        }

        protected override ISplitterFactory Construct(ILexiconFactory lexiconFactory)
        {
            return new StanfordSplitterFactory(
               Path.Combine(configuration.ResolvePath("Resources"), "Stanford"),
               lexiconFactory,
                cacheFactory);
        }
    }
}
