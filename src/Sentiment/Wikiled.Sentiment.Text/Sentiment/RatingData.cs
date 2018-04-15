using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Wikiled.Sentiment.Text.Sentiment
{
    [DataContract]
    public class RatingData : ICloneable
    {
        public bool HasValue => Positive > 0 || Negative > 0;

        [IgnoreDataMember]
        public bool IsPositive => RawRating > 0;

        public bool IsStrong
        {
            get
            {
                var difference = Positive - Negative;
                if (Math.Abs(difference) < 4)
                {
                    return false;
                }

                if (Positive == 0 ||
                    Negative == 0)
                {
                    return true;
                }

                var ratio = Positive > Negative ? Positive / Negative : Negative / Positive;
                return Math.Abs(ratio) >= 2;
            }
        }

        [DataMember]
        public double Negative { get; set; }

        [DataMember]
        public double Positive { get; set; }

        [DataMember]
        public double? RawRating
        {
            get => HasValue ? RatingCalculator.Calculate(Positive, Negative) : (double?)null;
            set
            {
            }
        }

        [DataMember]
        public double? StarsRating
        {
            get => RatingCalculator.CalculateStarsRating(RawRating);
            set
            {
            }
        }

        public static RatingData Accumulate(IEnumerable<RatingData> items)
        {
            var data = new RatingData();
            foreach (var ratingData in items)
            {
                data.Positive += ratingData.Positive;
                data.Negative += ratingData.Negative;
            }

            return data;
        }

        public static RatingData Accumulate(IEnumerable<SentimentValueData> items)
        {
            var data = new RatingData();
            foreach (var valueData in items)
            {
                data.AddSentiment(valueData);
            }

            return data;
        }

        public void AddSentiment(SentimentValueData data)
        {
            if (data.IsPositive)
            {
                Positive += data.Value;
            }
            else
            {
                Negative += -data.Value;
            }
        }

        public object Clone()
        {
            return new RatingData
            {
                Negative = Negative,
                Positive = Positive
            };
        }
    }
}
