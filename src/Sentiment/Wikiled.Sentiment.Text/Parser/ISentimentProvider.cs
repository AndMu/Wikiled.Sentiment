using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Parser
{
    public interface ISentimentProvider
    {
        SentimentValue MeasureSentiment(IWordItem word);
    }
}