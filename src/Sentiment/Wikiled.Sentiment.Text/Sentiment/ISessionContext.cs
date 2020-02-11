using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public interface ISessionContext
    {
        IAspectDectector Aspect { get; }

        bool DisableFeatureSentiment { get; }

        bool UseBuiltInSentiment { get; }

        bool DisableInvertors { get; }

        ISentimentDataHolder Lexicon { get; }

        string SvmPath { get; }
    }
}
