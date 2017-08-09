using Wikiled.Sentiment.Text.Data;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public interface IContextSentimentFactory
    {
        IContextSentiment Construct(IWordItemRelationships parent);
    }
}