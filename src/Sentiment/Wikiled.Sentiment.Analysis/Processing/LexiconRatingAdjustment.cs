using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class LexiconRatingAdjustment : BaseRatingAdjustment
    {
        private readonly ISentimentDataHolder sentimentData;

        public LexiconRatingAdjustment(IParsedReview review, ISentimentDataHolder sentimentData) 
            : base(review)
        {
            Guard.NotNull(() => sentimentData, sentimentData);
            this.sentimentData = sentimentData;
        }

        public override void CalculateRating()
        {
            foreach (var reviewItem in Review.Items)
            {
                var sentiment = sentimentData.MeasureSentiment(reviewItem);
                if (sentiment != null)
                {
                    Add(new SentimentValue(reviewItem,
                        new SentimentValueData(sentiment.DataValue.Value, SentimentSource.CustomAdjusted)));
                }
            }
        }
    }
}
