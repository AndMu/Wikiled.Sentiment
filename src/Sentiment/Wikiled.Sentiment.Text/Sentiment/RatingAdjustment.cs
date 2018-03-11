using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public class RatingAdjustment : IRatingAdjustment
    {
        private readonly Dictionary<IWordItem, SentimentValue> calculatedSentiments;

        public RatingAdjustment(IParsedReview review, IMachineSentiment model)
        {
            Guard.NotNull(() => review, review);
            Guard.NotNull(() => model, model);
            Model = model;
            Review = review;
            calculatedSentiments = new Dictionary<IWordItem, SentimentValue>(SimpleWordItemEquality.Instance);
            Rating = new RatingData();
            CalculateRating();
        }

        public IMachineSentiment Model { get; }

        public RatingData Rating { get; private set; }

        public IParsedReview Review { get; }

        public SentimentValue GetSentiment(IWordItem word)
        {
            calculatedSentiments.TryGetValue(word, out var value);
            return value;
        }

        private void CalculateRating()
        {
            var vector = Model.GetVector(Review.Vector.GetCells().ToArray());
            if (vector == null ||
                vector.Length == 0)
            {
                Rating = Review.CalculateRawRating();
                return;
            }

            var bias = vector.RHO;
            var rating = vector.Cells.FirstOrDefault(item => item.Data.Name == Constants.RATING_STARS);
            if (rating != null)
            {
                bias += rating.Calculated;
            }

            double added = Math.Abs(bias);
            foreach (var item in vector.Cells)
            {
                var cell = (TextVectorCell)item.Data;
                if (item.Data.Name != Constants.RATING_STARS &&
                    cell.Item != null)
                {
                    added += Math.Abs(item.Calculated);
                    Add(new SentimentValue(
                            (IWordItem)cell.Item,
                            new SentimentValueData(item.Calculated, SentimentSource.AdjustedSVM)));
                }
            }

            double totalSentiment = 0;
            double unknownSentiment = 0;
            List<SentimentValue> notAddedSentiments = new List<SentimentValue>();
            foreach (var sentimentValue in Review.GetAllSentiments())
            {
                double sentiment;
                if (!calculatedSentiments.ContainsKey(sentimentValue.Owner))
                {
                    sentiment = Math.Abs(sentimentValue.DataValue.Value);
                }
                else
                {
                    sentiment = Math.Abs(sentimentValue.DataValue.SentimentSource == SentimentSource.None ? sentimentValue.DataValue.Value / 2 : sentimentValue.DataValue.Value);
                    unknownSentiment += sentiment;
                    notAddedSentiments.Add(sentimentValue);
                }

                totalSentiment += sentiment;
            }

            var unknownWeight = unknownSentiment / totalSentiment;
            var weight = (1 - unknownWeight) * added;

            foreach (var sentiment in notAddedSentiments)
            {
                // unknown decrase45x
                var value = sentiment.DataValue.SentimentSource == SentimentSource.None ? sentiment.DataValue.Value / 2 : sentiment.DataValue.Value;
                Add(new SentimentValue(
                        sentiment.Owner,
                        new SentimentValueData(
                            value / weight / 2,
                            SentimentSource.AdjustedCalculated)));
            }

            if (calculatedSentiments.Count > 0)
            {
                Add(new SentimentValue(
                    WordOccurrence.CreateBasic(Constants.BIAS, POSTags.Instance.JJ),
                    new SentimentValueData(bias, SentimentSource.AdjustedSVM)));
            }
        }

        private void Add(SentimentValue sentiment)
        {
            if (calculatedSentiments.ContainsKey(sentiment.Owner))
            {
                return;
            }

            if (sentiment.Owner.Relationship.Inverted != null)
            {
                calculatedSentiments[sentiment.Owner.Relationship.Inverted] =
                    new SentimentValue(sentiment.Owner.Relationship.Inverted, new SentimentValueData(0, SentimentSource.AdjustedCanceled));
            }

            Rating.AddSentiment(sentiment.DataValue);
            calculatedSentiments[sentiment.Owner] = sentiment;
        }
    }
}
