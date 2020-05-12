using System.Collections.Generic;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Parser
{
    public interface ISentimentDataHolder
    {
        IEnumerable<WordSentimentValueData> Values { get; }

        void Merge(ISentimentDataHolder holder);

        SentimentValue MeasureSentiment(IWordItem word);

        double AverageStrength { get; }
    }
}