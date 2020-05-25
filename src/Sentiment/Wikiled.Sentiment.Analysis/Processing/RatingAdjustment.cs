using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Common.Logging;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class RatingAdjustment : BaseRatingAdjustment
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<RatingAdjustment>();

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
            TextVectorCell[] cells = Review.Vector.GetCells().ToArray();
            (double Probability, double Normalization, VectorData Vector) result = Model.GetVector(cells);
            VectorData vector = result.Vector;
            if (vector == null ||
                vector.Length == 0)
            {
                Rating = Review.CalculateRawRating();
                return;
            }

            var bias = vector.RHO;
            var fallbackWeight = 0.1;
            VectorCell lexicon = default;
            foreach (VectorCell item in vector.Cells)
            {
                var cell = (TextVectorCell)item.Data;
                if (cell.Name == Constants.RATING_STARS)
                {
                    lexicon = item;
                }

                if (cell.Item != null)
                {
                    var word = (IWordItem)cell.Item;
                    Add(new SentimentValue(word, word.Text, new SentimentValueData(item.Calculated, SentimentSource.AdjustedSVM)));
                }
                else
                {
                    bias += item.Calculated;
                }
            }

            var notAddedSentiments = new List<SentimentValue>();
            foreach (SentimentValue sentimentValue in Review.GetAllSentiments())
            {
                if (!ContainsSentiment(sentimentValue.Owner))
                {
                    notAddedSentiments.Add(sentimentValue);
                }
            }

            if (lexicon != null)
            {
                var totalWords = Review.GetAllSentiments().Length;
                fallbackWeight = Math.Abs(lexicon.Theta) / totalWords;
            }

            if (notAddedSentiments.Count > 0)
            {
                foreach (SentimentValue sentiment in notAddedSentiments)
                {
                    Add(new SentimentValue(sentiment.Owner, sentiment.Span, new SentimentValueData(sentiment.DataValue.Value * fallbackWeight, SentimentSource.AdjustedCalculated)));
                }
            }

            if (TotalSentiments > 0)
            {
                Add(new SentimentValue(
                    WordOccurrence.CreateBasic(Constants.BIAS, POSTags.Instance.JJ),
                    "BIAS",
                    new SentimentValueData(bias, SentimentSource.AdjustedSVM)));
            }

            if (Rating.HasValue)
            {
                if (Rating.IsPositive.Value &&
                    result.Probability < 0.5)
                {
                    log.LogDebug("Mistmatch in sentiment with machine prediction: {0} - {1}", Rating.IsPositive, result.Probability);
                }
            }
        }
    }
}
