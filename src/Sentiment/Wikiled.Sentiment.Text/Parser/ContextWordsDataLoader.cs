﻿using System;
using NLog;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Parser
{
    public class ContextWordsDataLoader : IContextWordsHandler
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IWordsHandler inner;

        public ContextWordsDataLoader(IWordsHandler inner, ISessionContext context)
        {
            log.Debug("Construct");
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

            bool value = Context.Aspect != null && Context.Aspect.IsAspect(word);
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
                var sentiment = Context.Lexicon.MeasureSentiment(word);
                if (sentiment != null ||
                    !Context.UseBuiltInSentiment)
                {
                    return sentiment;
                }
            }

            return inner.CheckSentiment(word);
        }
    }
}