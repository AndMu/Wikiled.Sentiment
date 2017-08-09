using System;
using System.Collections.Generic;
using Wikiled.Arff.Normalization;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Analysis.Review
{
    public class StrengthDetector
    {
        private readonly IMachineSentiment positive;

        private readonly IMachineSentiment negative;

        public StrengthDetector(IMachineSentiment positive, IMachineSentiment negative)
        {
            Guard.NotNull(() => positive, positive);
            Guard.NotNull(() => negative, negative);
            this.positive = positive;
            this.negative = negative;
        }

        public int Resolve(RatingData rating)
        {
            Guard.NotNull(() => rating, rating);
            IMachineSentiment machine = rating.Positive > rating.Negative
                                            ? positive
                                            : negative;

            List<TextVectorCell> cells = new List<TextVectorCell>();
            cells.Add(new TextVectorCell("POSITIVE", rating.Positive));
            cells.Add(new TextVectorCell("NEGATIVE", rating.Negative));
            var vector = machine.GetVector(cells, NormalizationType.L2);
            var result = machine.Predict(vector);
            if (result == null)
            {
                return 3;
            }

            var resultStrength = (SentimentStrength) (int) result;
            if (rating.Positive > rating.Negative)
            {
                switch (resultStrength)
                {
                    case SentimentStrength.Strong:
                        return 5;
                    case SentimentStrength.Medium:
                        return 4;
                    case SentimentStrength.Weak:
                        return 3;
                    default:
                        throw new ArgumentOutOfRangeException("rating");
                }
            }

            switch (resultStrength)
            {
                case SentimentStrength.Strong:
                    return 1;
                case SentimentStrength.Medium:
                    return 2;
                case SentimentStrength.Weak:
                    return 3;
                default:
                    throw new ArgumentOutOfRangeException("rating");
            }
        }
    }
}
