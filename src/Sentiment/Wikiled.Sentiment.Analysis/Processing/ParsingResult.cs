using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class ParsingResult
    {
        public ParsingResult(Document document, IParsedReview review)
        {
            Guard.NotNull(() => document, document);
            Guard.NotNull(() => review, review);
            Document = document;
            Review = review;
        }

        public Document Document { get; }

        public IParsedReview Review { get; }
    }
}
