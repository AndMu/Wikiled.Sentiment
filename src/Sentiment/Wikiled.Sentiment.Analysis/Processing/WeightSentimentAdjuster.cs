using System;
using System.IO;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class WeightSentimentAdjuster
    {
        private readonly ISentimentDataHolder sentimentDataHolder;

        public WeightSentimentAdjuster(ISentimentDataHolder sentimentDataHolder)
        {
            Guard.NotNull(() => sentimentDataHolder, sentimentDataHolder);
            this.sentimentDataHolder = sentimentDataHolder;
        }

        public double Multiplier { get; set; } = 1;

        public void Adjust(string weightFile)
        {
            Guard.NotNullOrEmpty(() => weightFile, weightFile);
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
