using System.Collections.Generic;
using System.Linq;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.MachineLearning;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public class SentimentValueData
    {
        private readonly double value;

        private readonly List<SentimentValueData> other = new List<SentimentValueData>();

        public SentimentValueData(double value, SentimentSource sentimentSource = SentimentSource.None)
        {
            this.value = value;
            SentimentSource = sentimentSource;
        }

        public bool IsPositive => Value > 0;

        public SentimentSource SentimentSource { get; }

        public double Value
        {
            get { return (value + other.Sum(item => item.value))/(1 + other.Count); }
        }

        public void Add(SentimentValueData value)
        {
            Guard.NotNull(() => value, value);
            other.Add(value);
        }

        public override string ToString()
        {
            return $"{Value:F2}, {SentimentSource}";
        }
    }
}
