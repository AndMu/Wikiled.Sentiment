using System.Collections.Generic;
using System.Linq;
using Wikiled.Sentiment.Text.MachineLearning;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public class SentimentValueData
    {
        private readonly List<SentimentValueData> other = new List<SentimentValueData>();

        private readonly double value;

        public SentimentValueData(double value, SentimentSource sentimentSource = SentimentSource.None)
        {
            this.value = value;
            SentimentSource = sentimentSource;
        }

        public bool IsPositive => Value > 0;

        public SentimentSource SentimentSource { get; }

        public double Value
        {
            get
            {
                return (value + other.Sum(item => item.value)) / (1 + other.Count);
            }
        }

        public override string ToString()
        {
            return $"{Value:F2}, {SentimentSource}";
        }

        public void Add(SentimentValueData value)
        {
            if (value == null)
            {
                throw new System.ArgumentNullException(nameof(value));
            }

            other.Add(value);
        }
    }
}
