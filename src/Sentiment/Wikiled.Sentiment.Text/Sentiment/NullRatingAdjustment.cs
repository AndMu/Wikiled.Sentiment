using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public class NullRatingAdjustment : IRatingAdjustment
    {
        public RatingData Rating => null;

        public SentimentValue GetSentiment(IWordItem word)
        {
            return word?.Relationship?.Sentiment;
        }
    }
}
