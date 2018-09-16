using Wikiled.Sentiment.Text.Aspects;

namespace Wikiled.Sentiment.Text.Configuration
{
    public interface ISentimentContext
    {
        IAspectDectector Aspect { get; }

        bool DisableFeatureSentiment { get; }

        bool DisableInvertors { get; }
    }
}
