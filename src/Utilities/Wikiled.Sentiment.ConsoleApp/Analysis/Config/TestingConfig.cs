using System.ComponentModel.DataAnnotations;

namespace Wikiled.Sentiment.ConsoleApp.Analysis.Config
{
    public class TestingConfig : BaseRawConfig
    {
        /// <summary>
        ///     Path to pretrained data. If empty will use as basic lexicon
        /// </summary>
        public string Model { get; set; }

        public bool Debug { get; set; }

        /// <summary>
        ///     Track Arff
        /// </summary>
        public bool TrackArff { get; set; }

        public bool ExtractStyle { get; set; }

        [Required]
        public string Out { get; set; }
    }
}
