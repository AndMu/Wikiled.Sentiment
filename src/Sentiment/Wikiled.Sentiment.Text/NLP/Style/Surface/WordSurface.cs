using System.Linq;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.NLP.Style.Description.Data;
using Wikiled.Sentiment.Text.Reflection;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.NLP.Style.Surface
{
    public class WordSurface : IDataSource
    {
        private readonly WordSurfaceData wordSurface = new WordSurfaceData();

        public WordSurface(TextBlock text)
        {
            Guard.NotNull(() => text, text);
            Text = text;
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

        public TextBlock Text { get; private set; }

        /// <summary>
        /// Average word length
        /// </summary>
        [InfoField("Average word length")]
        public double AverageLength
        {
            get
            {
                return wordSurface.AverageLength;
            }
        }

        /// <summary>
        /// Average number of syllables per word
        /// </summary>
        [InfoField("Average number of syllables per word")]
        public double AverageSyllables
        {
            get
            {
                return wordSurface.AverageSyllables;
            }
        }

        /// <summary>
        /// Percentage of all PureWords that have 3 or more syllables
        /// </summary>
        [InfoField("Percentage of PureWords with 3 or more syllables")]
        public double PercentWithManySyllables
        {
            get
            {
                return wordSurface.PercentWithManySyllables;
            }
        }
        /// <summary>
        /// Percentage of all PureWords that only have 1 syllable
        /// </summary>
        [InfoField("Percentage of all PureWords with 1 syllable")]
        public double PercentHavingOneSyllable
        {
            get
            {
                return wordSurface.PercentHavingOneSyllable;
            }
        }

        /// <summary>
        /// Percentage of all PureWords that have 6 or more letters
        /// </summary>
        [InfoField("Percentage of long PureWords")]
        public double PercentOfLong
        {
            get
            {
                return wordSurface.PercentOfLong;
            }
        }

        /// <summary>
        /// Percentage of word types divided by the number of word tokens
        /// </summary>
        [InfoField("Lemmas to tokens ratio")]
        public double PercentOfTypeByToken
        {
            get
            {
                return wordSurface.PercentOfTypeByToken;
            }
        }

        /// <summary>
        /// Percentage of PureWords that are subordinating conjunctions (then, until, while,
        /// since, etc.)
        /// </summary>
        [InfoField("Percentage of subordinating conjunctions")]
        public double PercentOfSubordinatingConjunctions
        {
            get
            {
                return wordSurface.PercentOfSubordinatingConjunctions;
            }
        }

        /// <summary>
        /// Percentage of PureWords that are coordinating conjunctions (but, so, but, or, etc.)
        /// </summary>
        [InfoField("Percentage of coordinating conjunctions")]
        public double PercentOfCoordinatingConjunctions
        {
            get
            {
                return wordSurface.PercentOfCoordinatingConjunctions;
            }
        }

        /// <summary>
        /// Percentage of PureWords that are articles
        /// </summary>
        [InfoField("Percentage of articles")]
        public double PercentOfArticles
        {
            get
            {
                return wordSurface.PercentOfArticles;
            }
        }
        /// <summary>
        /// Percentage of PureWords that are prepositions
        /// </summary>
        [InfoField("Percentage of prepositions")]
        public double PercentOfPrepositions
        {
            get
            {
                return wordSurface.PercentOfPrepositions;
            }
        }

        /// <summary>
        /// Percentage of PureWords that are pronouns
        /// </summary>
        [InfoField("Percentage of pronouns")]
        public double PercentOfPronouns
        {
            get
            {
                return wordSurface.PercentOfPronouns;
            }
        }

        /// <summary>
        /// Total syllables in text
        /// </summary>
        public int TotalSyllables
        {
            get
            {
                return wordSurface.TotalSyllables;
            }
        }

        public WordSurfaceData GetData()
        {
            return (WordSurfaceData)wordSurface.Clone();
        }
    }
}
