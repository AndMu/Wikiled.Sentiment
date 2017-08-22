using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Arff.Persistence;
using Wikiled.Core.Utility.Extensions;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.NLP.Inquirer;
using Wikiled.Sentiment.Text.NLP.NRC;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Reflection.Data;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Structure.Sentiment;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.Structure;

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
                int total;
                table.TryGetValue(wordEx.Text, out total);
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

        public static IParsedReview GetReview(this Document document, IWordsHandler manager)
        {
            return new ParsedReviewFactory(manager, document).Create();
        }

        public static IList<SentimentDataItem> GetSentenceSentimentData(this Document document)
        {
            var list = new List<SentimentDataItem>();
            foreach (var sentence in document.Sentences)
            {
                string text = sentence.Text;
                string tense = string.Empty;

                text += $" ({(tense.Length == 0 ? "Root" : tense)})";
                var item = new SentimentDataItem(
                    list.Count,
                    text,
                    sentence.CalculateSentiment(),
                    SentimentLevel.Sentence);
                item.ParentSentence = sentence;
                list.Add(item);
            }

            return list;
        }

        public static IList<double> GetSentimentCategoryDensity(
            this Document document,
            bool sentenceLevel,
            SentimentCategory category)
        {
            return GetConditionDensity(
                document,
                sentenceLevel,
                wordItem =>
                    {
                        var record = NRCDictionary.Instance.FindRecord(wordItem);
                        return record != null && record.HasValue(category);
                    });
        }

        public static IList<double> GetStyleDensity(this Document document, bool sentenceLevel, IDataItem leaf)
        {
            return GetStyleDensity(document, sentenceLevel, new[] { leaf });
        }

        public static IList<double> GetStyleDensity(this Document document, bool sentenceLevel, IDataTree tree)
        {
            return GetStyleDensity(document, sentenceLevel, tree?.AllLeafs.ToArray() ?? new IDataItem[] { });
        }

        public static IList<double> GetStyleDensity(this Document document, bool sentenceLevel, IDataItem[] leafs)
        {
            var table = leafs
                .Where(item => item != null)
                .Select(item => item.Name)
                .ToDictionary(item => item, StringComparer.OrdinalIgnoreCase);

            return GetConditionDensity(
                document,
                sentenceLevel,
                wordItem =>
                    {
                        if (leafs.Length == 0)
                        {
                            return false;
                        }

                        InquirerManager inquirer = InquirerManager.GetLoaded();
                        InquirerDefinition definition = inquirer.GetDefinitions(wordItem.Text);

                        return (from record in definition.Records
                                from category in record.RawCategories
                                where table.ContainsKey(category)
                                select category).Any();
                    });
        }

        public static IList<SentimentDataItem> GetWordSentimentData(this Document document)
        {
            var list = new List<SentimentDataItem>();
            foreach (var sentence in document.Sentences)
            {
                foreach (var wordEx in sentence.Words)
                {
                    string text = wordEx.Tag == null || wordEx.Tag == POSTags.Instance.UnknownWord
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

        public static void PopulateResults(this Document document, MachineDetectionResult result)
        {
            var itemTable = new Dictionary<string, List<WordEx>>(StringComparer.OrdinalIgnoreCase);
            foreach (var wordEx in document.Words)
            {
                wordEx.CalculatedValue = 0;
                itemTable.GetSafeCreate(wordEx.Text).Add(wordEx);
            }

            if (result == null)
            {
                return;
            }

            foreach (var vectorCell in result.Vector.Cells)
            {
                List<WordEx> words = itemTable[vectorCell.Data.Name];
                double eachWordValue = vectorCell.Calculated / words.Count;
                foreach (var wordEx in words)
                {
                    wordEx.CalculatedValue = eachWordValue;
                    wordEx.Theta = vectorCell.Theta;
                }
            }

            document.Stars = RatingCalculator.CalculateStarsRating(result.Result.Positivity);
        }
    }
}
