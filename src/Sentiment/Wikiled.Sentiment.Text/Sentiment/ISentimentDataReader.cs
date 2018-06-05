using System.Collections.Generic;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public interface ISentimentDataReader
    {
        IEnumerable<WordSentimentValueData> Read();
    }
}