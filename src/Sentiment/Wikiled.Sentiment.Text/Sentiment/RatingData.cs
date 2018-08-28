using System;
using System.Collections.Generic;
using Wikiled.MachineLearning.Mathematics;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public static class RatingDataExtension
    {
        public static RatingData Accumulate(this IEnumerable<SentimentValueData> items)
        {
            var data = new RatingData();
            foreach (var valueData in items)
            {
                data.AddSentiment(valueData);
            }

            return data;
        }

        public static void AddSentiment(this RatingData instance, SentimentValueData data)
        {
            if (data.IsPositive)
            {
                instance.AddPositive(Math.Abs(data.Value));
            }
            else
            {
                instance.AddNegative(Math.Abs(data.Value));
            }
        }
    }
}
