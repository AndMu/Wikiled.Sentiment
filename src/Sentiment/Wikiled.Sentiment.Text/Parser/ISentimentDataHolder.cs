using System.Collections.Generic;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Parser
{
    public interface ISentimentDataHolder
    {
        SentimentValue MeasureSentiment(IWordItem word);

        void SetValue(string word, SentimentValueData value);

        void Clear();

        Dictionary<string, SentimentValueData> CreateEmotionsData();

        void PopulateEmotionsData(Dictionary<string, double> data);
    }
}