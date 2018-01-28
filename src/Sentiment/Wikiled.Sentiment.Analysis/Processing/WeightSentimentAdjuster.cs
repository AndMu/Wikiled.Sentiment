using System;
using System.IO;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.MachineLearning;
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

            var lines = File.ReadAllLines(weightFile);
            foreach (var line in lines)
            {
                var items = line.Split(',');
                var word = items[0].Trim();
                var weight = double.Parse(items[1]);
                sentimentDataHolder.SetValue(word, new SentimentValueData(weight, SentimentSource.Word2Vec));
            }
        }
    }
}
