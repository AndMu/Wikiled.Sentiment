using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Parser
{
    public static class WordsHandlerExtension
    {
        public static bool IsKnown(this IWordsHandler instance, IWordItem word)
        {
            return instance.IsQuestion(word) ||
                   instance.IsFeature(word) ||
                   instance.IsInvertAdverb(word) ||
                   instance.IsSentiment(word) ||
                   instance.MeasureQuantifier(word) > 0;
        }

        public static SentimentValue MeasureSentiment(this IWordsHandler instance, IWordItem word)
        {
            if (instance.Context.DisableFeatureSentiment &&
                word.IsFeature)
            {
                return null;
            }

            return instance.CheckSentiment(word);
        }

        public static bool IsSentiment(this IWordsHandler instance, IWordItem word)
        {
            SentimentValue sentiment = instance.MeasureSentiment(word);
            return sentiment != null;
        }
    }
}
