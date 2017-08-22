using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
using System.Xml.Linq;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Helpers;
using Wikiled.Core.Utility.Serialization;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Text.Analysis.POS;
using Wikiled.Sentiment.Text.NLP.Repair;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Dictionary;
using Wikiled.Text.Analysis.NLP;
using Wikiled.Text.Analysis.NLP.Frequency;
using Wikiled.Text.Analysis.NLP.NRC;
using Wikiled.Text.Analysis.Resources;
using Wikiled.Text.Inquirer.Logic;

namespace Wikiled.Sentiment.Text.Parser
{
    public class WordsDataLoader : IWordsHandler
    {
        private readonly string datasetPath;

        private IAspectDectector aspectDectector;

        private Dictionary<string, double> booster;

        private Dictionary<string, double> negating;

        private Dictionary<string, WordRepairRule> negatingLemmaBasedRepair;

        private Dictionary<string, WordRepairRule> negatingRepairRule;

        private Dictionary<WordItemType, WordRepairRule> negatingRule;

        private Dictionary<string, double> question;

        private ISentenceRepairHandler repair;

        private Dictionary<string, double> stopPos;

        private Dictionary<string, double> stopWords;

        private readonly Lazy<INRCDictionary> nrcDictionary;

        private readonly Lazy<IInquirerManager> inquirerManager;

        public WordsDataLoader(string path, IWordsDictionary dictionary)
        {
            Guard.NotNullOrEmpty(() => path, path);
            datasetPath = path;
            AspectDectector = NullAspectDectector.Instance;
            Extractor = new RawWordExtractor(dictionary, MemoryCache.Default);
            WordFactory = new WordOccurenceFactory(this);
            AspectFactory = new MainAspectHandlerFactory(this);
            Reset();
            repair = new SentenceRepairHandler(Path.Combine(path, "Repair"), this);

            inquirerManager = new Lazy<IInquirerManager>(
                () =>
                {
                    var instance = new InquirerManager();
                    instance.Load();
                    return instance;
                });

            nrcDictionary = new Lazy<INRCDictionary>(
                () =>
                {
                    var instance = new NRCDictionary();
                    instance.Load();
                    return instance;
                });
        }

        public IInquirerManager InquirerManager => inquirerManager.Value;

        public INRCDictionary NRCDictionary => nrcDictionary.Value;

        public IAspectDectector AspectDectector
        {
            get => aspectDectector;
            set => aspectDectector = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IMainAspectHandlerFactory AspectFactory { get; }

        public bool DisableFeatureSentiment { get; set; }

        public bool DisableInvertors { get; set; }

        public IRawTextExtractor Extractor { get; }

        public bool IsDisableInvertorSentiment { get; set; }

        public IPOSTagger PosTagger { get; } = new NaivePOSTagger(new BNCList(), WordTypeResolver.Instance);

        public ISentenceRepairHandler Repair
        {
            get => repair ?? NullSentenceRepairHandler.Instance;
            set => repair = value;
        }

        public ISentimentDataHolder SentimentDataHolder { get; private set; }

        public IWordFactory WordFactory { get; }

        public WordRepairRule FindRepairRule(IWordItem word)
        {
            if (!word.IsSimple)
            {
                return null;
            }

            WordRepairRule rule;
            return negatingRepairRule.TryGetWordValue(word, out rule) ? rule : null;
        }

        public bool IsFeature(IWordItem word)
        {
            if (word.CanNotBeFeature())
            {
                return false;
            }

            bool value = AspectDectector != null && AspectDectector.IsAspect(word);
            return value;
        }

        public bool IsInvertAdverb(IWordItem word)
        {
            if (DisableInvertors)
            {
                return false;
            }

            if (negating.ContainsKey(word.Text))
            {
                WordRepairRule rule;
                if (negatingRule.TryGetValue(WordItemType.Invertor, out rule))
                {
                    var value = new WordRepairRuleEngine(word, rule).Evaluate();
                    if (value.HasValue)
                    {
                        return value.Value;
                    }
                }

                return true;
            }
            
            var evaluationValue = new WordRepairRuleEngine(word, FindRepairRule(word)).Evaluate();
            if (evaluationValue.HasValue)
            {
                return evaluationValue.Value;
            }

            return false;
        }

        public bool IsKnown(IWordItem word)
        {
            return IsQuestion(word) ||
                   IsFeature(word) ||
                   IsInvertAdverb(word) ||
                   IsSentiment(word) ||
                   MeasureQuantifier(word) > 0;
        }

        public bool IsQuestion(IWordItem word)
        {
            return question.ContainsKey(word.Text);
        }

        public bool IsSentiment(IWordItem word)
        {
            var sentiment = MeasureSentiment(word);
            return sentiment != null;
        }

        public bool IsStop(IWordItem wordItem)
        {
            if (wordItem.Text.Length <= 1 ||
                stopWords.ContainsKey(wordItem.Text) ||
                stopPos.ContainsKey(wordItem.POS.Tag))
            {
                return true;
            }

            return false;
        }

        public void Load()
        {
            SentimentDataHolder.Clear();
            booster = ReadTextData("BoosterWordList.txt", false);
            negating = ReadTextData("NegatingWordList.txt", true);
            question = ReadTextData("QuestionWords.txt", true);
            stopWords = ReadTextData("StopWords.txt", true);
            stopPos = ReadTextData("StopPos.txt", true);
            SentimentDataHolder.PopulateEmotionsData(ReadTextData("EmotionLookupTable.txt", false));
            ReadRepairRules();
        }

        public double? MeasureQuantifier(IWordItem word)
        {
            double value;
            if (booster.TryGetValue(word.Text, out value))
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

            if (DisableFeatureSentiment &&
                word.IsFeature)
            {
                return null;
            }

            return SentimentDataHolder.MeasureSentiment(word);
        }

        public void Reset()
        {
            SentimentDataHolder = new SentimentDataHolder();
            Load();
        }

        public void Save()
        {
            WriteData("BoosterWordList.txt", false, booster);
            WriteData("NegatingWordList.txt", true, negating);
            WriteData("QuestionWords.txt", true, question);
            WriteData("StopWords.txt", true, stopWords);
            WriteData("StopPos.txt", true, stopPos);
            WriteData("EmotionLookupTable.txt", true, SentimentDataHolder.CreateEmotionsData());
        }

        private void ReadRepairRules()
        {
            string folder = Path.Combine(datasetPath, @"Rules\Invertors");
            negatingLemmaBasedRepair = new Dictionary<string, WordRepairRule>(StringComparer.OrdinalIgnoreCase);
            negatingRepairRule = new Dictionary<string, WordRepairRule>(StringComparer.OrdinalIgnoreCase);
            negatingRule = new Dictionary<WordItemType, WordRepairRule>();
            foreach (var file in Directory.GetFiles(folder))
            {
                var data = XDocument.Load(file).XmlDeserialize<WordRepairRule>();
                if (!string.IsNullOrEmpty(data.Lemma))
                {
                    negatingLemmaBasedRepair[data.Lemma] = data;
                }

                if (!string.IsNullOrEmpty(data.Word))
                {
                    negatingRepairRule[data.Word] = data;
                }

                if (data.Type != WordItemType.None)
                {
                    negatingRule[data.Type] = data;
                }
            }
        }

        private Dictionary<string, double> ReadTextData(string file, bool useDefault)
        {
            return ReadTabResourceDataFile.ReadTextData(Path.Combine(datasetPath, file), useDefault);
        }

        private void WriteData<T1, T2>(string file, bool useDefault, Dictionary<T1, T2> data)
        {
            string path = Path.Combine(datasetPath, @"Out");
            using (var boosterData = new WriteTabResourceDataFile(Path.Combine(path, file)))
            {
                boosterData.UseDefaultIfNotFound = useDefault;
                boosterData.WriteData(data);
            }
        }
    }
}
