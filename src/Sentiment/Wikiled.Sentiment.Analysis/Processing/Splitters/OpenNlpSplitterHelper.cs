using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.NLP.OpenNLP;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Text.Analysis.Cache;

namespace Wikiled.Sentiment.Analysis.Processing.Splitters
{
    public class OpenNlpSplitterHelper : BaseSplitterHelper
    {
        private readonly ICacheFactory cacheFactory;

        private readonly IConfigurationHandler configuration;

        public OpenNlpSplitterHelper(ICacheFactory cacheFactory, IConfigurationHandler configuration, int parallel = 0)
            : base(configuration, parallel)
        {
            Guard.NotNull(() => cacheFactory, cacheFactory);
            Guard.NotNull(() => configuration, configuration);
            this.cacheFactory = cacheFactory;
            this.configuration = configuration;
        }

        protected override ISplitterFactory Construct(ILexiconFactory lexiconFactory)
        {
            return new OpenNlpSplitterFactory(configuration.ResolvePath("Resources"), lexiconFactory, cacheFactory);
        }
    }
}
