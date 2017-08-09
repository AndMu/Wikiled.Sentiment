using System;
using NLog;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public static class RatingCalculator
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static int? CalculateStar(double? value)
        {
            var stars = CalculateStarsRating(value);
            if (!stars.HasValue)
            {
                return null;
            }

            return (int)Math.Round(stars.Value);
        }

        public static double? CalculateStarsRating(double? value)
        {
            return (value + 1) / 2 * 4 + 1;
        }

        public static double? ConvertToRaw(double star)
        {
            return ((star - 1) / 4 * 2) - 1;
        }

        public static double Calculate(double positive, double negative)
        {
            const int coefficient = 2;
            if (positive == 0 &&
                negative == 0)
            {
                return 0;
            }

            const double min = 0.0001;
            positive += min;
            negative += min;
            double rating = Math.Log(positive / negative, 2);

            if (positive == min ||
                rating < -coefficient)
            {
                rating = -coefficient;
            }
            else if (negative == min || rating > coefficient)
            {
                rating = coefficient;
            }

            rating = rating / coefficient;
            log.Debug("Positive: {0} Negative: {1} Rating: {2}", positive, negative, rating);
            return rating;
        }
    }
}
