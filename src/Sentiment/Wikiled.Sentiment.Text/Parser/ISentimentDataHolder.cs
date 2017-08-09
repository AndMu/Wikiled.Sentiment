using System.Collections.Generic;
using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Text.Parser
{
    public interface ISentimentDataHolder
        : ISentimentProvider
    {
        void AddValue(string word, SentimentValueData value);

        void Clear();

        Dictionary<string, SentimentValueData> CreateEmotionsData();

        void PopulateEmotionsData(Dictionary<string, double> data);

        bool Adjust(string word, double value);
    }
}