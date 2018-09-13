﻿using Autofac;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Text.Analysis.POS;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.NLP;

namespace Wikiled.Sentiment.Text.Parser
{
    public interface IWordsHandler
    {
        bool DisableInvertors { get; set; }

        IContainer Container { get; }

        IMainAspectHandlerFactory AspectFactory { get; }

        IPOSTagger PosTagger { get; }

        ISentimentDataHolder SentimentDataHolder { get; }

        IWordFactory WordFactory { get; }

        IRawTextExtractor Extractor { get; }
         
        IAspectDectector AspectDectector { get; set; }

        bool DisableFeatureSentiment { get; set; }

        void Reset();

        bool IsFeature(IWordItem word);

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
