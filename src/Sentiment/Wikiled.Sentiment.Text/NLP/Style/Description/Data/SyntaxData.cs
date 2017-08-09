namespace Wikiled.Sentiment.Text.NLP.Style.Description.Data
{
    public class SyntaxData : INLPDataItem
    {
        /// <summary>
        /// Percentage of words that are adjective
        /// </summary>
        public double AdjectivesPercentage { get; set; }

        /// <summary>
        ///  Percentage of words that are adverbs
        /// </summary>
        public double AdverbsPercentage { get; set; }

        /// <summary>
        /// Percentage of words that are interrogative words (who, what, where when, etc.)
        /// </summary>
        public double QuestionPercentage { get; set; }

        /// <summary>
        /// Percentage of words that are nouns
        /// </summary>
        public double NounsPercentage { get; set; }

        /// <summary>
        /// Percentage of words that are verbs
        /// </summary>
        public double VerbsPercentage { get; set; }

        /// <summary>
        /// Ratio of number of adjectives to nouns
        /// </summary>
        public double AdjectivesToNounsRatio { get; set; }

        /// <summary>
        /// Percentage of words that are proper nouns
        /// </summary>
        public double ProperNounsPercentage { get; set; }

        /// <summary>
        ///  Percentage of words that are numbers (i.e. cardinal, ordinal, nouns such as
        /// dozen, thousands, etc.)
        /// </summary>
        public double NumbersPercentage { get; set; }

        /// <summary>
        /// Diversity of POS tri-grams
        /// </summary>
        public double POSDiversity { get; set; }

        public object Clone()
        {
            return new SyntaxData
            {
                AdjectivesPercentage = AdjectivesPercentage,
                AdjectivesToNounsRatio = AdjectivesToNounsRatio,
                AdverbsPercentage = AdverbsPercentage,
                NounsPercentage = NounsPercentage,
                NumbersPercentage = NumbersPercentage,
                POSDiversity = POSDiversity,
                ProperNounsPercentage = ProperNounsPercentage,
                QuestionPercentage = QuestionPercentage,
                VerbsPercentage = VerbsPercentage
            };
        }
    }
}
