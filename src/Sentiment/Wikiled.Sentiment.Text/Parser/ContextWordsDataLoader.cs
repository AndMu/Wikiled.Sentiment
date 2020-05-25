using Microsoft.Extensions.Logging;
using System;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Parser
{
    public class ContextWordsDataLoader : IContextWordsHandler
    {
        private readonly ILogger<ContextWordsDataLoader> log;

        private readonly IWordsHandler inner;

        public ContextWordsDataLoader(ILogger<ContextWordsDataLoader> log, IWordsHandler inner, ISessionContext context)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
            log.LogDebug("Construct");
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ISessionContext Context { get; }

        public bool IsFeature(IWordItem word)
        {
            if (word.CanNotBeFeature())
            {
                return false;
            }

            var value = Context.Aspect != null && Context.Aspect.IsAspect(word);
            return value;
        }

        public bool IsAttribute(IWordItem word)
        {
            return Context.Aspect.IsAttribute(word);
        }

        public bool IsInvertAdverb(IWordItem word)
        {
            if (Context.DisableInvertors)
            {
                return false;
            }

            return inner.IsInvertAdverb(word);
        }

        public bool IsQuestion(IWordItem word)
        {
            return inner.IsQuestion(word);
        }

        public bool IsStop(IWordItem word)
        {
            return inner.IsStop(word);
        }

        public double? MeasureQuantifier(IWordItem word)
        {
            return inner.MeasureQuantifier(word);
        }

        public SentimentValue CheckSentiment(IWordItem word)
        {
            if (Context.DisableFeatureSentiment &&
                word.IsFeature)
            {
                return null;
            }

            if (Context.Lexicon != null)
            {
                SentimentValue sentiment = Context.Lexicon.MeasureSentiment(word);
                if (sentiment != null ||
                    !Context.UseBuiltInSentiment)
                {
                    return sentiment;
                }

                var builtIn = inner.CheckSentiment(word);
                if (builtIn != null)
                {
                    // original max strength is 3 we move it save scale as custom and also make it 4 time less powerfull
                    return new SentimentValue(word, builtIn.Span, builtIn.DataValue.Value / 3 * Context.Lexicon.AverageStrength / 4);
                }
                else
                {
                    return null;
                }
            }

            return inner.CheckSentiment(word);
        }
    }
}
