using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Common.Extensions;
using Wikiled.Sentiment.Text.Helpers;
using Wikiled.Sentiment.Text.NLP.Style.Description.Data;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.Reflection;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.NLP.Style
{
    public class SyntaxFeatures : IDataSource
    {
        private readonly SyntaxData data = new SyntaxData();

        private readonly Dictionary<string, int> posTables = new Dictionary<string, int>();

        private readonly IWordsHandler wordsHandler;

        public SyntaxFeatures(IWordsHandler wordsHandler, TextBlock text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            this.wordsHandler = wordsHandler ?? throw new ArgumentNullException(nameof(wordsHandler));
        }

        /// <summary>
        ///     Percentage of words that are adjective
        /// </summary>
        [InfoField("Adjectives Percentage")]
        public double AdjectivesPercentage => data.AdjectivesPercentage;

        /// <summary>
        ///     Ratio of number of adjectives to nouns
        /// </summary>
        [InfoField("Adjectives To NounsRatio")]
        public double AdjectivesToNounsRatio => data.AdjectivesToNounsRatio;

        /// <summary>
        ///     Percentage of words that are adverbs
        /// </summary>
        [InfoField("Adverbs Percentage")]
        public double AdverbsPercentage => data.AdverbsPercentage;

        /// <summary>
        ///     Percentage of words that are nouns
        /// </summary>
        [InfoField("Nouns Percentage")]
        public double NounsPercentage => data.NounsPercentage;

        /// <summary>
        ///     Percentage of words that are numbers (i.e. cardinal, ordinal, nouns such as
        ///     dozen, thousands, etc.)
        /// </summary>
        [InfoField("Numbers Percentage")]
        public double NumbersPercentage => data.NumbersPercentage;

        /// <summary>
        ///     Diversity of POS tri-grams
        /// </summary>
        [InfoField("Diversity of POS tri-grams")]
        public double POSDiversity => data.POSDiversity;

        /// <summary>
        ///     Percentage of words that are proper nouns
        /// </summary>
        [InfoField("Proper Nouns Percentage")]
        public double ProperNounsPercentage => data.ProperNounsPercentage;

        /// <summary>
        ///     Percentage of words that are interrogative words (who, what, where when, etc.)
        /// </summary>
        [InfoField("Questions Percentage")]
        public double QuestionPercentage => data.QuestionPercentage;

        public TextBlock Text { get; }

        /// <summary>
        ///     Percentage of words that are verbs
        /// </summary>
        [InfoField("Verbs Percentage")]
        public double VerbsPercentage => data.VerbsPercentage;

        public SyntaxData GetData()
        {
            return (SyntaxData)data.Clone();
        }

        public void Load()
        {
            foreach (var sentenceItem in Text.Sentences)
            {
                AutoEvictingDictionary<WordEx, IWordItem> table =
                    new AutoEvictingDictionary<WordEx, IWordItem>(length: 3);
                foreach (var word in sentenceItem.Words)
                {
                    table.Increment();
                    if (!(word.UnderlyingWord is IWordItem wordItem) ||
                        wordItem.POS.WordType == WordType.Symbol ||
                        wordItem.POS.WordType == WordType.SeparationSymbol ||
                        wordItem.POS.WordType == WordType.Unknown ||
                        !wordItem.POS.Tag.HasLetters())
                    {
                        continue;
                    }

                    table.Add(word, wordItem);
                    if (table.Count == 3)
                    {
                        string mask = $"{table.Values[0].POS.Tag} {table.Values[1].POS.Tag} {table.Values[2].POS.Tag}";
                        if (posTables.TryGetValue(mask, out int total))
                        {
                            total += 1;
                        }
                        else
                        {
                            total = 1;
                        }

                        posTables[mask] = total;
                    }
                }
            }

            data.AdjectivesPercentage = (double)Text.Words.Count(item => wordsHandler.PosTagger.IsWordType(item, WordType.Adjective)) /
                                        Text.Words.Length;
            data.AdverbsPercentage = (double)Text.Words.Count(item => wordsHandler.PosTagger.IsWordType(item, WordType.Adverb)) / Text.Words.Length;
            data.QuestionPercentage = (double)Text.Words.Count(item => item.IsQuestion(wordsHandler)) / Text.Words.Length;
            data.NounsPercentage = (double)Text.Words.Count(item => wordsHandler.PosTagger.IsWordType(item, WordType.Noun)) / Text.Words.Length;
            data.VerbsPercentage = (double)Text.Words.Count(item => wordsHandler.PosTagger.IsWordType(item, WordType.Verb)) /
                                   Text.Words.Length;
            var nouns = (double)Text.Words.Count(item => wordsHandler.PosTagger.IsWordType(item, WordType.Noun));
            if (nouns == 0)
            {
                data.AdjectivesToNounsRatio = 0;
            }
            else
            {
                data.AdjectivesToNounsRatio = Text.Words.Count(item => wordsHandler.PosTagger.IsWordType(item, WordType.Adjective)) /
                                              nouns;
            }

            data.ProperNounsPercentage = (double)Text.Words.Count(item => wordsHandler.PosTagger.IsWordType(item, POSTags.Instance.NNS)) /
                                         Text.Words.Length;
            data.NumbersPercentage = (double)Text.Words.Count(item => item.IsDigit()) /
                                     Text.Words.Length;
            if (posTables.Count == 0)
            {
                data.POSDiversity = 0;
            }
            else
            {
                data.POSDiversity = (double)posTables.Count /
                                    posTables.Sum(item => item.Value);
            }
        }
    }
}
