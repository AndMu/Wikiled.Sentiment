using Autofac;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Wikiled.Common.Serialization;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.NLP.Repair;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Dictionary;
using Wikiled.Text.Analysis.Dictionary.Streams;
using Wikiled.Text.Analysis.NLP;
using Wikiled.Text.Analysis.NLP.Frequency;
using Wikiled.Text.Analysis.NLP.NRC;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.Words;
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

        public WordsDataLoader(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(path));
            }

            datasetPath = path;
            Reset();
        }

        public IAspectDectector AspectDectector { get => aspectDectector; set => aspectDectector = value ?? throw new ArgumentNullException(nameof(value)); }

        public IPOSTagger PosTagger { get; } = new NaivePOSTagger(new BNCList(), WordTypeResolver.Instance);

        public IContainer Container { get; private set; }

        public IMainAspectHandlerFactory AspectFactory { get; private set; }

        public bool DisableFeatureSentiment { get; set; }

        public bool DisableInvertors { get; set; }

        public IRawTextExtractor Extractor { get; private set; }

        public IFrequencyListManager FrequencyListManager { get; private set; }

        public bool IsDisableInvertorSentiment { get; set; }

        public ISentenceRepairHandler Repair { get => repair ?? NullSentenceRepairHandler.Instance; set => repair = value; }

        public ISentimentDataHolder SentimentDataHolder { get; private set; }

        public IWordFactory WordFactory { get; private set; }

        public WordRepairRule FindRepairRule(IWordItem word)
        {
            if (!word.IsSimple)
            {
                return null;
            }

            return negatingRepairRule.TryGetWordValue(word, out WordRepairRule rule) ? rule : null;
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
                if (negatingRule.TryGetValue(WordItemType.Invertor, out WordRepairRule rule))
                {
                    bool? value = new WordRepairRuleEngine(word, rule).Evaluate();
                    if (value.HasValue)
                    {
                        return value.Value;
                    }
                }

                return true;
            }

            bool? evaluationValue = new WordRepairRuleEngine(word, FindRepairRule(word)).Evaluate();
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
            SentimentValue sentiment = MeasureSentiment(word);
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
            booster = ReadTextData("BoosterWordList.txt");
            negating = ReadTextData("NegatingWordList.txt");
            question = ReadTextData("QuestionWords.txt");
            stopWords = ReadTextData("StopWords.txt");
            stopPos = ReadTextData("StopPos.txt");
            SentimentDataHolder.PopulateEmotionsData(ReadTextData("EmotionLookupTable.txt"));
            ReadRepairRules();
        }

        public double? MeasureQuantifier(IWordItem word)
        {
            if (booster.TryGetValue(word.Text, out double value))
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
            if (DisableFeatureSentiment &&
                word.IsFeature)
            {
                return null;
            }

            return SentimentDataHolder.MeasureSentiment(word);
        }

        public void Reset()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<BasicEnglishDictionary>().As<IWordsDictionary>().SingleInstance();
            builder.RegisterType<InquirerManager>().As<IInquirerManager>().SingleInstance().OnActivated(item => item.Instance.Load());
            builder.RegisterType<NRCDictionary>().As<INRCDictionary>().SingleInstance().OnActivated(item => item.Instance.Load());
            Container = builder.Build();

            SentimentDataHolder = new SentimentDataHolder();
            AspectDectector = NullAspectDectector.Instance;
            Extractor = new RawWordExtractor(Container.Resolve<IWordsDictionary>(), new MemoryCache(new MemoryCacheOptions()));
            WordFactory = new WordOccurenceFactory(this);
            AspectFactory = new MainAspectHandlerFactory(this);
            FrequencyListManager = new FrequencyListManager();

            Load();

            repair = new SentenceRepairHandler(Path.Combine(datasetPath, "Repair"), this);
        }

        private void ReadRepairRules()
        {
            string folder = Path.Combine(datasetPath, @"Rules/Invertors");
            negatingLemmaBasedRepair = new Dictionary<string, WordRepairRule>(StringComparer.OrdinalIgnoreCase);
            negatingRepairRule = new Dictionary<string, WordRepairRule>(StringComparer.OrdinalIgnoreCase);
            negatingRule = new Dictionary<WordItemType, WordRepairRule>();
            foreach (string file in Directory.GetFiles(folder))
            {
                WordRepairRule data = XDocument.Load(file).XmlDeserialize<WordRepairRule>();
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

        private Dictionary<string, double> ReadTextData(string file)
        {
            DictionaryStream stream = new DictionaryStream(Path.Combine(datasetPath, file), new FileStreamSource());
            return stream.ReadDataFromStream(double.Parse).ToDictionary(item => item.Word, item => item.Value, StringComparer.OrdinalIgnoreCase);
        }
    }
}
