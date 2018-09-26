using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Parser
{
    public interface IWordsHandler
    {
        bool IsFeature(IWordItem word);

        bool IsAttribute(IWordItem word);

        bool IsInvertAdverb(IWordItem word);

        bool IsQuestion(IWordItem word);

        bool IsStop(IWordItem wordItem);

        double? MeasureQuantifier(IWordItem word);

        SentimentValue CheckSentiment(IWordItem word);
    }
}
