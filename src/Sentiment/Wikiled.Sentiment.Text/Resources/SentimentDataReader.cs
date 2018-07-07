using System;
using System.Collections.Generic;
using System.IO;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Text.Resources
{
    public class SentimentDataReader : ISentimentDataReader
    {
        private readonly string weightFile;

        public SentimentDataReader(string weightFile)
        {
            if (string.IsNullOrWhiteSpace(weightFile))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(weightFile));
            }

            this.weightFile = weightFile;
        }

        public IEnumerable<WordSentimentValueData> Read()
        {
            foreach (var line in File.ReadLines(weightFile))
            {
                var items = line.Split(',');
                var word = items[0].Trim();
                var weight = double.Parse(items[1]);
                yield return new WordSentimentValueData(word, new SentimentValueData(weight, SentimentSource.Word2Vec));
            }
        }
    }
}
