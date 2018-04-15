using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public class RatingAdjustment : IRatingAdjustment
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

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
            var cells = Review.Vector.GetCells().ToArray();
            var result = Model.GetVector(cells);
            var vector = result.Vector;
            if (vector == null ||
                vector.Length == 0)
            {
                Rating = Review.CalculateRawRating();
                return;
            }

            var bias = vector.RHO;
            double added = Math.Abs(bias);
            foreach (var item in vector.Cells)
            {
                var cell = (TextVectorCell)item.Data;
                if (cell.Item != null)
                {
                    added += Math.Abs(item.Calculated);
                    Add(new SentimentValue(
                            (IWordItem)cell.Item,
                            new SentimentValueData(item.Calculated, SentimentSource.AdjustedSVM)));
                }
                else
                {
                    bias += item.Calculated;
                }
            }

            List<SentimentValue> notAddedSentiments = new List<SentimentValue>();
            foreach (var sentimentValue in Review.GetAllSentiments())
            {
                if (!calculatedSentiments.ContainsKey(sentimentValue.Owner))
                {
                    notAddedSentiments.Add(sentimentValue);
                }
            }

            var weight = 0.25 * vector.Normalization.Coeficient;
            if (notAddedSentiments.Count > 0)
            {
                foreach (var sentiment in notAddedSentiments)
                {
                    Add(new SentimentValue(sentiment.Owner, new SentimentValueData(sentiment.DataValue.Value * weight, SentimentSource.AdjustedCalculated)));
                }
            }

            if (calculatedSentiments.Count > 0)
            {
                Add(new SentimentValue(
                    WordOccurrence.CreateBasic(Constants.BIAS, POSTags.Instance.JJ),
                    new SentimentValueData(bias, SentimentSource.AdjustedSVM)));
            }

            if (Rating.IsPositive &&
                result.Probability < 0.5)
            {
                log.Debug("Mistmatch in sentiment with machine prediction: {0} - {1}", Rating.IsPositive, result.Probability);
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
