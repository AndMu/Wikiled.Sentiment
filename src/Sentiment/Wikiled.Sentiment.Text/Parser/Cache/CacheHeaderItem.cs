using System;
using System.Xml.Serialization;
using Wikiled.Sentiment.Text.Persitency;

namespace Wikiled.Sentiment.Text.Parser.Cache
{
    [XmlRoot("DataItem")]
    public class CacheHeaderItem : IIndexRegistry
    {
        public string File { get; set; }

        public int Length { get; set; }

        public string Beggining { get; set; }

        public string Ending { get; set; }

        public DateTime Date { get; set; }

        public string Tag { get; set; }
    }
}
