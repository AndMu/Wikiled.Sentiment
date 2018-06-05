using System.Collections.Generic;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class BaseRatingAdjustment : IRatingAdjustment
    {
        public BaseRatingAdjustment(IParsedReview review)
        {
            Guard.NotNull(() => review, review);
            Review = review;
            CalculatedSentiments = new Dictionary<IWordItem, SentimentValue>(SimpleWordItemEquality.Instance);
            Rating = new RatingData();
        }

        protected Dictionary<IWordItem, SentimentValue> CalculatedSentiments { get; }

        public RatingData Rating { get; protected set; }

        public IParsedReview Review { get; }

        public SentimentValue GetSentiment(IWordItem word)
        {
            CalculatedSentiments.TryGetValue(word, out var value);
            return value;
        }

        protected void Add(SentimentValue sentiment)
        {
            if (CalculatedSentiments.ContainsKey(sentiment.Owner))
            {
                return;
            }

            if (sentiment.Owner.Relationship.Inverted != null)
            {
                CalculatedSentiments[sentiment.Owner.Relationship.Inverted] = new SentimentValue(
                    sentiment.Owner.Relationship.Inverted, new SentimentValueData(0, SentimentSource.AdjustedCanceled));
            }

            Rating.AddSentiment(sentiment.DataValue);
            CalculatedSentiments[sentiment.Owner] = sentiment;
        }
    }
}