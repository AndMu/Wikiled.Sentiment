using System;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Parser
{
    public class CustomWordsDataLoader : IWordsHandler
    {
        private readonly IWordsHandler inner;

        private readonly ISentimentDataHolder sentimentData;

        public CustomWordsDataLoader(IWordsHandler inner, ISentimentDataHolder sentimentData)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
            this.sentimentData = sentimentData ?? throw new ArgumentNullException(nameof(sentimentData));
        }

        public ISentimentContext Context => inner.Context;

        public bool IsFeature(IWordItem word)
        {
            return inner.IsFeature(word);
        }

        public bool IsAttribute(IWordItem word)
        {
            return inner.IsAttribute(word);
        }

        public bool IsInvertAdverb(IWordItem word)
        {
            return inner.IsInvertAdverb(word);
        }

        public bool IsQuestion(IWordItem word)
        {
            return inner.IsInvertAdverb(word);
        }
   
        public bool IsStop(IWordItem wordItem)
        {
            return inner.IsStop(wordItem);
        }

        public double? MeasureQuantifier(IWordItem word)
        {
            return inner.MeasureQuantifier(word);
        }

        public SentimentValue CheckSentiment(IWordItem word)
        {
            return sentimentData.MeasureSentiment(word);
        }
    }
}
