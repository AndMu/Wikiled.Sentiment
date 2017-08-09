using System.Xml.Serialization;

namespace Wikiled.Sentiment.Text.Features
{
    public class DetectionBlock
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("extractable")]
        public bool Extractable { get; set; }

        [XmlArray("words")]
        [XmlArrayItem("word")]
        public DetectionItem[] Words { get; set; }

        public override string ToString()
        {
            return $"DetectionBlock [{Name}] with {Words.Length} words";
        }
    }
}
