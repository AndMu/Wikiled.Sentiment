using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Resources;

namespace Wikiled.Sentiment.Analysis.Processing.Splitters
{
    public class SimpleSplitterHelper : BaseSplitterHelper
    {
        public SimpleSplitterHelper(IConfigurationHandler configuration, int parallel = 0)
            : base(configuration, parallel)
        {
        }

        protected override ISplitterFactory Construct(ILexiconFactory lexiconFactory)
        {
            return new SimpleSplitterFactory(lexiconFactory);
        }
    }
}
