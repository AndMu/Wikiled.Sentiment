using System.Xml.Serialization;

namespace Wikiled.Sentiment.Text.Aspects.Data
{
    public class AspectData
    {
        [XmlArray("Aspects")]
        [XmlArrayItem("Aspect")]
        public string[] Aspects { get; set; }

        [XmlArray("Attributes")]
        [XmlArrayItem("Attribute")]
        public string[] Attributes { get; set; }
    }
}
