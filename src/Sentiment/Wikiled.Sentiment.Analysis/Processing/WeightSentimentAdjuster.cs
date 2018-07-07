using System;
using System.IO;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Resources;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class WeightSentimentAdjuster
    {
        private readonly ISentimentDataHolder sentimentDataHolder;

        public WeightSentimentAdjuster(ISentimentDataHolder sentimentDataHolder)
        {
            this.sentimentDataHolder = sentimentDataHolder ?? throw new ArgumentNullException(nameof(sentimentDataHolder));
        }

        public double Multiplier { get; set; } = 1;

        public void Adjust(string weightFile)
        {
            if (string.IsNullOrEmpty(weightFile))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(weightFile));
            }

            if (!File.Exists(weightFile))
            {
                throw new ArgumentOutOfRangeException(nameof(weightFile), weightFile);
            }

            foreach (var line in new SentimentDataReader(weightFile).Read())
            {
                sentimentDataHolder.SetValue(line.Word, line.Data);
            }
        }
    }
}
