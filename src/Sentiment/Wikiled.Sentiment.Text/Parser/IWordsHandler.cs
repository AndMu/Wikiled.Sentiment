using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Parser
{
    public interface IWordsHandler
    {
        ISentimentContext Context { get; }

        bool IsFeature(IWordItem word);

        bool IsAttribute(IWordItem word);

        bool IsInvertAdverb(IWordItem word);

        bool IsKnown(IWordItem word);

        bool IsQuestion(IWordItem word);

        bool IsSentiment(IWordItem word);

        bool IsStop(IWordItem wordItem);

        void Load();

        double? MeasureQuantifier(IWordItem word);

        SentimentValue MeasureSentiment(IWordItem word);
    }
}
