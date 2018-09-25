using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.NLP
{
    public interface IParsedReviewManagerFactory
    {
        IParsedReviewManager Resolve(Document document, ISentimentDataHolder lexicon = null);
    }
}