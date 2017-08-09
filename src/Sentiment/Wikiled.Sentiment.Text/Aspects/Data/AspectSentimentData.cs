using System.Xml.Serialization;

namespace Wikiled.Sentiment.Text.Aspects.Data
{
    public class AspectSentimentData
    {
        [XmlArray("Aspects")]
        [XmlArrayItem("Aspect")]
        public AspectSentimentItem[] Records { get; set; }

        public int TotalReviews { get; set; }
    }
}
