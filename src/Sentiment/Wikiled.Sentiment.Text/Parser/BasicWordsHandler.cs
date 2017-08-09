using System;
using System.Runtime.Caching;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.NLP.Repair;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Dictionary;
using Wikiled.Text.Analysis.NLP;
using Wikiled.Text.Analysis.POS;
using WordsDictionary = Wikiled.Sentiment.Text.Helpers.WordsDictionary;

namespace Wikiled.Sentiment.Text.Parser
{
    public class BasicWordsHandler : IWordsHandler
    {
        private IAspectDectector aspectDectector;

        private WordsDictionary invertors;

        private WordsDictionary quantifiers;

        private WordsDictionary questions;

        private SentimentDataHolder sentiment;

        private WordsDictionary stop;

        public BasicWordsHandler(IPOSTagger posTagger)
        {
            Guard.NotNull(() => posTagger, posTagger);
            PosTagger = posTagger;
            WordFactory = new WordOccurenceFactory(this);
            AspectFactory = new MainAspectHandlerFactory(this);
            Reset();
        }

        public IAspectDectector AspectDectector
        {
            get => aspectDectector;
            set => aspectDectector = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IMainAspectHandlerFactory AspectFactory { get; }

        public bool DisableFeatureSentiment { get; set; }

        public bool DisableInvertors { get; set; }

        public IRawTextExtractor Extractor { get; } = new RawWordExtractor(BasicEnglishDictionary.Instance, MemoryCache.Default);

        public bool IsDisableInvertorSentiment { get; set; }

        public IPOSTagger PosTagger { get; }

        public ISentenceRepairHandler Repair { get; } = NullSentenceRepairHandler.Instance;

        public ISentimentDataHolder SentimentDataHolder => sentiment;

        public IWordFactory WordFactory { get; }

        public WordRepairRule FindRepairRule(IWordItem word)
        {
            return null;
        }

        public bool IsFeature(IWordItem word)
        {
            return AspectDectector != null && AspectDectector.IsAspect(word);
        }

        public bool IsInvertAdverb(IWordItem word)
        {
            return !DisableInvertors && invertors.Contains(word.Text);
        }

        public bool IsKnown(IWordItem word)
        {
            return IsFeature(word) || IsStop(word) || IsQuestion(word) || IsSentiment(word) || IsInvertAdverb(word);
        }

        public bool IsQuestion(IWordItem word)
        {
            return questions.Contains(word.Text);
        }

        public bool IsSentiment(IWordItem word)
        {
            return sentiment.MeasureSentiment(word) != null;
        }

        public bool IsStop(IWordItem word)
        {
            return stop.Contains(word.Text);
        }

        public void Load()
        {
        }

        public double? MeasureQuantifier(IWordItem word)
        {
            double value;
            if (quantifiers.RawData.TryGetValue(word.Text, out value))
            {
                if (value == 0)
                {
                    return 1.5;
                }

                if (value > 0)
                {
                    return value + 0.5;
                }

                return 1 / (-value + 0.5);
            }

            return null;
        }

        public SentimentValue MeasureSentiment(IWordItem word)
        {
            if (word.IsStopWord ||
                !word.IsSimple)
            {
                return null;
            }

            return sentiment.MeasureSentiment(word);
        }

        public void Reset()
        {
            sentiment = new SentimentDataHolder();
            invertors = WordsDictionary.ConstructFromInternalStream(@"Resources.Dictionary.InvertorWords.txt");
            stop = WordsDictionary.ConstructFromInternalStream(@"Resources.Dictionary.StopWords.txt");
            questions = WordsDictionary.ConstructFromInternalStream(@"Resources.Dictionary.QuestionWords.txt");
            var sentiments = WordsDictionary.ConstructFromInternalStream(@"Resources.Dictionary.Sentiments.txt");
            quantifiers = WordsDictionary.ConstructFromInternalStream(@"Resources.Dictionary.Quantifiers.txt");
            AspectDectector = NullAspectDectector.Instance;
            sentiment.PopulateEmotionsData(sentiments.RawData);
        }
    }
}
