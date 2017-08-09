using System.Collections.Generic;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public interface IContextSentiment
    {
        IList<SentimentValue> Sentiments { get; }
    }
}