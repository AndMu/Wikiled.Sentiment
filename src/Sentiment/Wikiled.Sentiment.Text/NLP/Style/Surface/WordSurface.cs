using System;
using System.Linq;
using Wikiled.Sentiment.Text.NLP.Style.Description.Data;
using Wikiled.Text.Analysis.Reflection;
using Wikiled.Text.Analysis.Words;

namespace Wikiled.Sentiment.Text.NLP.Style.Surface
{
    public class WordSurface : IDataSource
    {
        private readonly WordSurfaceData wordSurface = new WordSurfaceData();

        public WordSurface(TextBlock text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        /// <summary>
        ///     Average word length
        /// </summary>
        [InfoField("Average word length")]
        public double AverageLength => wordSurface.AverageLength;

        /// <summary>
        ///     Average number of syllables per word
        /// </summary>
        [InfoField("Average number of syllables per word")]
        public double AverageSyllables => wordSurface.AverageSyllables;

        /// <summary>
        ///     Percentage of all PureWords that only have 1 syllable
        /// </summary>
        [InfoField("Percentage of all PureWords with 1 syllable")]
        public double PercentHavingOneSyllable => wordSurface.PercentHavingOneSyllable;

        /// <summary>
        ///     Percentage of PureWords that are articles
        /// </summary>
        [InfoField("Percentage of articles")]
        public double PercentOfArticles => wordSurface.PercentOfArticles;

        /// <summary>
        ///     Percentage of PureWords that are coordinating conjunctions (but, so, but, or, etc.)
        /// </summary>
        [InfoField("Percentage of coordinating conjunctions")]
        public double PercentOfCoordinatingConjunctions => wordSurface.PercentOfCoordinatingConjunctions;

        /// <summary>
        ///     Percentage of all PureWords that have 6 or more letters
        /// </summary>
        [InfoField("Percentage of long PureWords")]
        public double PercentOfLong => wordSurface.PercentOfLong;

        /// <summary>
        ///     Percentage of PureWords that are prepositions
        /// </summary>
        [InfoField("Percentage of prepositions")]
        public double PercentOfPrepositions => wordSurface.PercentOfPrepositions;

        /// <summary>
        ///     Percentage of PureWords that are pronouns
        /// </summary>
        [InfoField("Percentage of pronouns")]
        public double PercentOfPronouns => wordSurface.PercentOfPronouns;

        /// <summary>
        ///     Percentage of PureWords that are subordinating conjunctions (then, until, while,
        ///     since, etc.)
        /// </summary>
        [InfoField("Percentage of subordinating conjunctions")]
        public double PercentOfSubordinatingConjunctions => wordSurface.PercentOfSubordinatingConjunctions;

        /// <summary>
        ///     Percentage of word types divided by the number of word tokens
        /// </summary>
        [InfoField("Lemmas to tokens ratio")]
        public double PercentOfTypeByToken => wordSurface.PercentOfTypeByToken;

        /// <summary>
        ///     Percentage of all PureWords that have 3 or more syllables
        /// </summary>
        [InfoField("Percentage of PureWords with 3 or more syllables")]
        public double PercentWithManySyllables => wordSurface.PercentWithManySyllables;

        public TextBlock Text { get; }

        /// <summary>
        ///     Total syllables in text
        /// </summary>
        public int TotalSyllables => wordSurface.TotalSyllables;

        public WordSurfaceData GetData()
        {
            return (WordSurfaceData)wordSurface.Clone();
        }

        public void Load()
        {
            wordSurface.AverageLength = (double)Text.PureWords.Sum(item => item.Text.Length) / Text.PureWords.Length;
            wordSurface.AverageSyllables = (double)Text.PureWords.Sum(item => item.CountSyllables()) / Text.PureWords.Length;
            wordSurface.PercentWithManySyllables = (double)Text.PureWords.Count(item => item.CountSyllables() >= 3) / Text.PureWords.Length;
            wordSurface.PercentHavingOneSyllable = (double)Text.PureWords.Count(item => item.CountSyllables() == 1) / Text.PureWords.Length;
            wordSurface.PercentOfLong = (double)Text.PureWords.Count(item => item.Text.Length >= 6) / Text.PureWords.Length;
            wordSurface.PercentOfTypeByToken = (double)Text.TotalLemmas / Text.TotalWordTokens;
            wordSurface.PercentOfSubordinatingConjunctions = (double)Text.PureWords.Count(item => WordTypeResolver.Instance.IsSubordinateConjunction(item.Text)) /
                                                             Text.PureWords.Length;
            wordSurface.PercentOfCoordinatingConjunctions = (double)Text.PureWords.Count(item => item.IsCoordinatingConjunction()) / Text.PureWords.Length;
            wordSurface.PercentOfArticles = (double)Text.PureWords.Count(item => WordTypeResolver.Instance.IsArticle(item.Text)) / Text.PureWords.Length;
            wordSurface.PercentOfPrepositions = (double)Text.PureWords.Count(item => WordTypeResolver.Instance.IsPreposition(item.Text)) / Text.PureWords.Length;
            wordSurface.PercentOfPronouns = (double)Text.PureWords.Count(item => item.IsPronoun()) / Text.PureWords.Length;
            wordSurface.TotalSyllables = Text.PureWords.Sum(item => item.CountSyllables());
        }
    }
}
