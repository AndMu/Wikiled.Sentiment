using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Wikiled.Common.Serialization;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.NLP.Repair;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Dictionary.Streams;

namespace Wikiled.Sentiment.Text.Parser
{
    public class WordsHandler : IWordsHandler
    {
        private readonly ILexiconConfiguration config;

        private Dictionary<string, double> booster;

        private bool loaded;

        private Dictionary<string, double> negating;

        private Dictionary<string, WordRepairRule> negatingLemmaBasedRepair;

        private Dictionary<string, WordRepairRule> negatingRepairRule;

        private Dictionary<WordItemType, WordRepairRule> negatingRule;

        private Dictionary<string, double> question;

        private ISentimentDataHolder sentimentData;

        private Dictionary<string, double> stopPos;

        private Dictionary<string, double> stopWords;

        private IExtendedWords extended;

        public WordsHandler(ILexiconConfiguration config, IExtendedWords extended)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.extended = extended ?? throw new ArgumentNullException(nameof(extended));
            sentimentData = new SentimentDataHolder();
        }

        public SentimentValue CheckSentiment(IWordItem word)
        {
            return sentimentData.MeasureSentiment(word);
        }

        public bool IsAttribute(IWordItem word)
        {
            return false;
        }

        public bool IsFeature(IWordItem word)
        {
            return false;
        }

        public bool IsInvertAdverb(IWordItem word)
        {
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

        public bool IsQuestion(IWordItem word)
        {
            return question.ContainsKey(word.Text);
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
            if (loaded)
            {
                throw new InvalidOperationException();
            }

            loaded = true;
            booster = ReadTextData("BoosterWordList.txt");
            negating = ReadTextData("NegatingWordList.txt");
            question = ReadTextData("QuestionWords.txt");
            stopWords = ReadTextData("StopWords.txt");
            stopPos = ReadTextData("StopPos.txt");
            var emotions = ReadTextData("EmotionLookupTable.txt");
            foreach (var sentiment in extended.GetSentiments())
            {
                if (!emotions.ContainsKey(sentiment.Word))
                {
                    emotions[sentiment.Word] = sentiment.Sentiment;
                }
            }

            sentimentData = SentimentDataHolder.PopulateEmotionsData(emotions);
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

        private WordRepairRule FindRepairRule(IWordItem word)
        {
            if (!word.IsSimple)
            {
                return null;
            }

            return negatingRepairRule.TryGetWordValue(word, out WordRepairRule rule) ? rule : null;
        }

        private void ReadRepairRules()
        {
            string folder = Path.Combine(config.LexiconPath, @"Rules/Invertors");
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
            DictionaryStream stream = new DictionaryStream(Path.Combine(config.LexiconPath, file), new FileStreamSource());
            return stream.ReadDataFromStream(double.Parse).ToDictionary(item => item.Word, item => item.Value, StringComparer.OrdinalIgnoreCase);
        }
    }
}
