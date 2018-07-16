using Wikiled.MachineLearning.Mathematics;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public interface IRatingAdjustment
    {
        RatingData Rating { get; }

        SentimentValue GetSentiment(IWordItem word);

        IParsedReview Review { get; }
    }
}