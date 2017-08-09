namespace Wikiled.Sentiment.Text.NLP.Style.Description.Data
{
    public class CharactersSurfaceData : INLPDataItem
    {
        /// <summary>
        /// Percentage of all characters that are punctuation characters
        /// </summary>
        public double PercentOfPunctuation { get; set; }

        /// <summary>
        /// Percentage of all characters that are semicolons
        /// </summary>
        public double PercentOfSemicolons { get; set; }

        /// <summary>
        /// Percentage of all characters that are commas
        /// </summary>
        public double PercentOfCommas { get; set; }

        public object Clone()
        {
            return new CharactersSurfaceData
            {
                PercentOfCommas = PercentOfCommas,
                PercentOfPunctuation = PercentOfPunctuation,
                PercentOfSemicolons = PercentOfSemicolons
            };
        }
    }
}
