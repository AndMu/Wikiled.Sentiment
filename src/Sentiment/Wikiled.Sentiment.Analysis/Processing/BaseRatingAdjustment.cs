using System.Collections.Generic;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public abstract class BaseRatingAdjustment : IRatingAdjustment
    {
        private readonly Dictionary<IWordItem, SentimentValue> calculatedSentiments;

        protected BaseRatingAdjustment(IParsedReview review)
        {
            Review = review ?? throw new System.ArgumentNullException(nameof(review));
            calculatedSentiments = new Dictionary<IWordItem, SentimentValue>(SimpleWordItemEquality.Instance);
            Rating = new RatingData();
        }

        public int TotalSentiments => calculatedSentiments.Count;

        public RatingData Rating { get; protected set; }

        public IParsedReview Review { get; }

        public SentimentValue GetSentiment(IWordItem word)
        {
            calculatedSentiments.TryGetValue(word, out var value);
            return value;
        }

        public abstract void CalculateRating();

        protected bool ContainsSentiment(IWordItem word)
        {
            return calculatedSentiments.ContainsKey(word);
        }

        protected void Add(SentimentValue sentiment)
        {
            if (calculatedSentiments.ContainsKey(sentiment.Owner))
            {
                return;
            }

            if (sentiment.Owner.Relationship.Inverted != null)
            {
                calculatedSentiments[sentiment.Owner.Relationship.Inverted] = 
                    new SentimentValue(
                        sentiment.Owner.Relationship.Inverted, 
                        new SentimentValueData(0, SentimentSource.AdjustedCanceled));
            }

            Rating.AddSentiment(sentiment.DataValue);
            calculatedSentiments[sentiment.Owner] = sentiment;
        }
    }
}