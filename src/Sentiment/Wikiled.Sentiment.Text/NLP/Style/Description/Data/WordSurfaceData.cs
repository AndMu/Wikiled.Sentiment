namespace Wikiled.Sentiment.Text.NLP.Style.Description.Data
{
    public class WordSurfaceData : INLPDataItem
    {
        /// <summary>
        /// Average word length
        /// </summary>
        public double AverageLength { get; set; }

        /// <summary>
        /// Average number of syllables per word
        /// </summary>
        public double AverageSyllables { get; set; }

        /// <summary>
        /// Percentage of all PureWords that have 3 or more syllables
        /// </summary>
        public double PercentWithManySyllables { get; set; }

        /// <summary>
        /// Percentage of all PureWords that only have 1 syllable
        /// </summary>
        public double PercentHavingOneSyllable { get; set; }

        /// <summary>
        /// Percentage of all PureWords that have 6 or more letters
        /// </summary>
        public double PercentOfLong { get; set; }

        /// <summary>
        /// Percentage of word types divided by the number of word tokens
        /// </summary>
        public double PercentOfTypeByToken { get; set; }

        /// <summary>
        /// Percentage of PureWords that are subordinating conjunctions (then, until, while,
        /// since, etc.)
        /// </summary>
        public double PercentOfSubordinatingConjunctions { get; set; }

        /// <summary>
        /// Percentage of PureWords that are coordinating conjunctions (but, so, but, or, etc.)
        /// </summary>
        public double PercentOfCoordinatingConjunctions { get; set; }

        /// <summary>
        /// Percentage of PureWords that are articles
        /// </summary>
        public double PercentOfArticles { get; set; }

        /// <summary>
        /// Percentage of PureWords that are prepositions
        /// </summary>
        public double PercentOfPrepositions { get; set; }

        /// <summary>
        /// Percentage of PureWords that are pronouns
        /// </summary>
        public double PercentOfPronouns { get; set; }

        /// <summary>
        /// Total syllables in text
        /// </summary>
        public int TotalSyllables { get; set; }

        public object Clone()
        {
            return new WordSurfaceData()
            {
                AverageLength = AverageLength,
                AverageSyllables = AverageSyllables,
                PercentHavingOneSyllable = PercentHavingOneSyllable,
                PercentOfArticles = PercentOfArticles,
                PercentOfCoordinatingConjunctions = PercentOfCoordinatingConjunctions,
                PercentOfLong = PercentOfLong,
                PercentOfPrepositions = PercentOfPrepositions,
                PercentOfPronouns = PercentOfPronouns,
                PercentOfSubordinatingConjunctions = PercentOfSubordinatingConjunctions,
                PercentOfTypeByToken = PercentOfTypeByToken,
                PercentWithManySyllables = PercentWithManySyllables,
                TotalSyllables = TotalSyllables
            };
        }
    }
}
