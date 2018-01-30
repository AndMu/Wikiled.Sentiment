using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.NLP
{
    public class ParsedReviewManagerFactory : IParsedReviewManagerFactory
    {
        public IParsedReviewManager Create(IWordsHandler manager, Document document)
        {
            return new ParsedReviewManager(manager, document);
        }
    }
}
