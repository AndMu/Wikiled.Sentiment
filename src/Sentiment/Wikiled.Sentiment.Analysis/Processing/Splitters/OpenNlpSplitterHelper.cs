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
            this.cacheFactory = cacheFactory ?? throw new System.ArgumentNullException(nameof(cacheFactory));
            this.configuration = configuration ?? throw new System.ArgumentNullException(nameof(configuration));
        }

        protected override ISplitterFactory Construct(ILexiconFactory lexiconFactory)
        {
            return new RecyclableSplitterFactory(new OpenNlpSplitterFactory(configuration.ResolvePath("Resources"), lexiconFactory, cacheFactory));
        }
    }
}
