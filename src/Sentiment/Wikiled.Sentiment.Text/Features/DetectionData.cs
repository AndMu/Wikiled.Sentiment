using System.Xml.Serialization;

namespace Wikiled.Sentiment.Text.Features
{
    [XmlRoot("features")]
    public class DetectionData
    {
        [XmlArray("adjectives")]
        [XmlArrayItem("adjective")]
        public DetectionBlock[] Adjectives { get; set; }
    }
}
