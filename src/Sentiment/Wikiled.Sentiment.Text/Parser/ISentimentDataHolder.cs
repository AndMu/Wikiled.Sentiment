using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Parser
{
    public interface ISentimentDataHolder
    {
        SentimentValue MeasureSentiment(IWordItem word);

        double AverageStrength { get; }
    }
}