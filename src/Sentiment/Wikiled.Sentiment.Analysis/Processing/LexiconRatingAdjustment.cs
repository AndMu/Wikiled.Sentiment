using System;
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
            if (review is null)
            {
                throw new ArgumentNullException(nameof(review));
            }

            this.sentimentData = sentimentData ?? throw new ArgumentNullException(nameof(sentimentData));
        }

        protected override void CalculateRatingLogic()
        {
            foreach (var reviewItem in Review.ImportantWords)
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
