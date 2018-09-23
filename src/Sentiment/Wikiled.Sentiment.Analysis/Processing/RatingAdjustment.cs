using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class RatingAdjustment : BaseRatingAdjustment
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private RatingAdjustment(IParsedReview review, IMachineSentiment model)
            : base(review)
        {
            if (review is null)
            {
                throw new ArgumentNullException(nameof(review));
            }

            Model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public IMachineSentiment Model { get; }

        public static IRatingAdjustment Create(IParsedReview review, IMachineSentiment model)
        {
            if (model is NullMachineSentiment)
            {
                return new NullRatingAdjustment(review);
            }

            var adjustment = new RatingAdjustment(review, model);
            adjustment.CalculateRating();
            return adjustment;
        }

        protected override void CalculateRatingLogic()
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
            foreach (var item in vector.Cells)
            {
                var cell = (TextVectorCell)item.Data;
                if (cell.Item != null)
                {
                    Add(new SentimentValue((IWordItem)cell.Item, new SentimentValueData(item.Calculated, SentimentSource.AdjustedSVM)));
                }
                else
                {
                    bias += item.Calculated;
                }
            }

            List<SentimentValue> notAddedSentiments = new List<SentimentValue>();
            foreach (var sentimentValue in Review.GetAllSentiments())
            {
                if (!ContainsSentiment(sentimentValue.Owner))
                {
                    notAddedSentiments.Add(sentimentValue);
                }
            }

            var weight = 0.25 / vector.Normalization.Coeficient;
            if (notAddedSentiments.Count > 0)
            {
                foreach (var sentiment in notAddedSentiments)
                {
                    Add(new SentimentValue(sentiment.Owner, new SentimentValueData(sentiment.DataValue.Value * weight, SentimentSource.AdjustedCalculated)));
                }
            }

            if (TotalSentiments > 0)
            {
                Add(new SentimentValue(
                    WordOccurrence.CreateBasic(Constants.BIAS, POSTags.Instance.JJ),
                    new SentimentValueData(bias, SentimentSource.AdjustedSVM)));
            }

            if (Rating.HasValue)
            {
                if (Rating.IsPositive.Value &&
                    result.Probability < 0.5)
                {
                    log.Debug("Mistmatch in sentiment with machine prediction: {0} - {1}", Rating.IsPositive, result.Probability);
                }
            }
        }
    }
}
