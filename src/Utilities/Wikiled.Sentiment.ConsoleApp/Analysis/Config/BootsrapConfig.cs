using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Wikiled.Sentiment.ConsoleApp.Analysis.Config
{
    public class BootsrapConfig : BaseRawConfig
    {
        [Description("Minimum sentiment words occurence")]
        public int Minimum { get; set; } = 3;

        [Required]
        public string Destination { get; set; }

        public double? BalancedTop { get; set; }

        public string Model { get; set; }
    }
}
