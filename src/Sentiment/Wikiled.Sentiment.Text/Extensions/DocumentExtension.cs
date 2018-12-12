using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.Structure.Sentiment;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.Structure;
using PositivityType = Wikiled.Arff.Persistence.PositivityType;

namespace Wikiled.Sentiment.Text.Extensions
{
    public static class DocumentExtension
    {
        public static IList<double> GetAspectDensity(this Document document, bool sentenceLevel)
        {
            return GetConditionDensity(document, sentenceLevel, wordItem => wordItem.IsAspect);
        }

        public static IList<TextVectorCell> GetCellsOccurenceOnly(this Document document)
        {
            var table = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var itemTable = new Dictionary<string, IItem>(StringComparer.OrdinalIgnoreCase);
            foreach (var wordEx in document.Words)
            {
                table.TryGetValue(wordEx.Text, out var total);
                total++;
                table[wordEx.Text] = total;
                itemTable[wordEx.Text] = wordEx.UnderlyingWord;
                wordEx.Value = 1;
            }

            return table.Select(item => new TextVectorCell(itemTable[item.Key], item.Key, item.Value))
                        .ToList();
        }

        public static IList<double> GetConditionDensity(
            this Document document,
            bool sentenceLevel,
            Func<WordEx, bool> condition)
        {
            var list = new List<double>();
            foreach (var sentence in document.Sentences)
            {
                double current = 0;
                foreach (var wordEx in sentence.Words)
                {
                    if (condition(wordEx))
                    {
                        current++;
                    }

                    if (!sentenceLevel)
                    {
                        list.Add(current);
                        current = 0;
                    }
                }

                if (sentenceLevel)
                {
                    list.Add(current);
                }
            }

            return list;
        }

        public static PositivityType? GetPositivity(this Document document)
        {
            if (document?.Stars == null)
            {
                return null;
            }

            return document.Stars > 3 ? PositivityType.Positive : PositivityType.Negative;
        }

        public static IList<SentimentDataItem> GetSentenceSentimentData(this Document document)
        {
            var list = new List<SentimentDataItem>();
            foreach (var sentence in document.Sentences)
            {
                var text = sentence.Text;
                var tense = string.Empty;

                text += $" ({(tense.Length == 0 ? "Root" : tense)})";
                var item = new SentimentDataItem(
                    list.Count,
                    text,
                    sentence.CalculateSentiment().RawRating,
                    SentimentLevel.Sentence);
                item.ParentSentence = sentence;
                list.Add(item);
            }

            return list;
        }

        public static IList<SentimentDataItem> GetWordSentimentData(this Document document)
        {
            var list = new List<SentimentDataItem>();
            foreach (var sentence in document.Sentences)
            {
                foreach (var wordEx in sentence.Words)
                {
                    var text = wordEx.Tag == null || wordEx.Tag == POSTags.Instance.UnknownWord
                                      ? wordEx.Text
                                      : $"{wordEx.Text} ({wordEx.Type})";

                    var item = new SentimentDataItem(
                        list.Count,
                        text,
                        wordEx.CalculatedValue,
                        SentimentLevel.Word);

                    item.ParentSentence = sentence;
                    item.ParentWord = wordEx;
                    if (wordEx.IsAspect)
                    {
                        item.Border = 3;
                    }

                    list.Add(item);
                }
            }

            return list;
        }
    }
}
