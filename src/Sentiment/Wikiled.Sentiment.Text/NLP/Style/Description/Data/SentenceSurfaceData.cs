namespace Wikiled.Sentiment.Text.NLP.Style.Description.Data
{
    public class SentenceSurfaceData : INLPDataItem
    {
        /// <summary>
        /// Average sentence length
        /// </summary>
        public double AverageLength { get; set; }

        /// <summary>
        /// Percentage of long sentences (sentences greater than 15 words)
        /// </summary>
        public double PercentOfLong { get; set; }

        /// <summary>
        /// Percentage of short sentences (sentences less than 8 words)
        /// </summary>
        public double PercentOfShort { get; set; }

        /// <summary>
        /// Percentage of sentences that are questions
        /// </summary>
        public double PercentOfQuestion { get; set; }

        /// <summary>
        /// Percentage of sentences that begin with a subordinating or coordinating conjunctions
        /// </summary>
        public double PercentOfBeginningWithConjunction { get; set; }

        public object Clone()
        {
            return new SentenceSurfaceData
            {
                AverageLength = AverageLength,
                PercentOfBeginningWithConjunction = PercentOfBeginningWithConjunction,
                PercentOfLong = PercentOfLong,
                PercentOfQuestion = PercentOfQuestion,
                PercentOfShort = PercentOfShort
            };
        }
    }
}
