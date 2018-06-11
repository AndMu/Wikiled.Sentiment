using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Structure
{
    public interface IDocumentFromReviewFactory
    {
        Document ReparseDocument(IRatingAdjustment adjustment);
    }
}