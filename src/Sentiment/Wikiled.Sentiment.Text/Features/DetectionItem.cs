using System.Xml.Serialization;

namespace Wikiled.Sentiment.Text.Features
{
    public class DetectionItem
    {
        [XmlIgnore]
        public DetectionBlock Block { get; set; }

        [XmlElement("text")]
        public string Text { get; set; }

        [XmlElement("probability")]
        public double Probability { get; set; }

        public override string ToString()
        {
            return $"DetectionItem: {Text} [{Probability}] int {Block}";
        }
    }
}
