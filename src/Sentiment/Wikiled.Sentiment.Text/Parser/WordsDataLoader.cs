﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Wikiled.Common.Serialization;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.NLP.Repair;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Dictionary.Streams;

namespace Wikiled.Sentiment.Text.Parser
{
    public class WordsDataLoader : IWordsHandler
    {
        private readonly ILexiconConfiguration config;

        private ISentimentContext context;

        private Dictionary<string, double> booster;

        private Dictionary<string, double> negating;

        private Dictionary<string, WordRepairRule> negatingLemmaBasedRepair;

        private Dictionary<string, WordRepairRule> negatingRepairRule;

        private Dictionary<WordItemType, WordRepairRule> negatingRule;

        private Dictionary<string, double> question;

        private Dictionary<string, double> stopPos;

        private Dictionary<string, double> stopWords;

        private bool loaded;

        public WordsDataLoader(ILexiconConfiguration config, ISentimentContext context)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            SentimentData = new SentimentDataHolder();
        }

        public ISentimentDataHolder SentimentData { get; private set; }

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

            bool value = context.Aspect != null && context.Aspect.IsAspect(word);
            return value;
        }

        public bool IsAttribute(IWordItem word)
        {
            return context.Aspect.IsAttribute(word);
        }

        public bool IsInvertAdverb(IWordItem word)
        {
            if (context.DisableInvertors)
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
            SentimentData = SentimentDataHolder.PopulateEmotionsData(ReadTextData("EmotionLookupTable.txt"));
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
            if (context.DisableFeatureSentiment &&
                word.IsFeature)
            {
                return null;
            }

            return SentimentData.MeasureSentiment(word);
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
