using Wikiled.MachineLearning.Mathematics;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public class NullRatingAdjustment : IRatingAdjustment
    {
        public NullRatingAdjustment(IParsedReview review)
        {
            Review = review ?? throw new System.ArgumentNullException(nameof(review));
            Rating = review.CalculateRawRating();
        }

        public RatingData Rating { get; }

        public IParsedReview Review { get; }

        public void CalculateRating()
        {
        }

        public SentimentValue GetSentiment(IWordItem word)
        {
            return word?.Relationship?.Sentiment;
        }
    }
}
