namespace Wikiled.Sentiment.ConsoleApp.Analysis.Config
{
    public class TrainingConfig : BaseRawConfig
    {
        /// <summary>
        /// Path to Feautres/Aspects
        /// </summary>
        public string Features { get; set; }

        /// <summary>
        /// Do you want to use all words of filter using threshold (min 3 reviews with words and words with 10 occurrences)
        /// </summary>
        public bool UseAll { get; set; }

        public string Model { get; set; } = @".\Svm";
    }
}
