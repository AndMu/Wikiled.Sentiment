using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public interface IRatingAdjustment
    {
        RatingData Rating { get; }

        SentimentValue GetSentiment(IWordItem word);

        Document GenerateDocument();
    }
}