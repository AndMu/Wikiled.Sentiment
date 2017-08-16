using Wikiled.Core.Utility.Resources;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.NLP;

namespace Wikiled.Sentiment.Analysis.Processing.Splitters
{
    public class SimpleSplitterHelper : BaseSplitterHelper
    {
        public SimpleSplitterHelper(ConfigurationHandler configuration, int parallel = 0)
            : base(configuration, parallel)
        {
        }

        protected override ISplitterFactory Construct(ILexiconFactory lexiconFactory)
        {
            return new SimpleSplitterFactory(lexiconFactory);
        }
    }
}
