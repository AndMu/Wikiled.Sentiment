using System;
using System.Xml.Serialization;

namespace Wikiled.Sentiment.Text.Data.Review
{
    public class SingleProcessingData
    {
        public SingleProcessingData()
        {
        }

        public SingleProcessingData(string text)
            : this()
        {
            Text = string.IsNullOrEmpty(text) ? "<Empty>" : text.Trim();
        }

        [XmlAttribute]
        public DateTime Date { get; set; }

        [XmlAttribute]
        public double? Stars { get; set; }

        [XmlAttribute]
        public string Author { get; set; }

        [XmlAttribute]
        public string Id { get; set; }

        public string Text { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Text))
            {
                return "<EMPTY>";
            }

            if (Text.Length <= 100)
            {
                return Text;
            }

            return Text.Substring(0, 100).Replace("\r", string.Empty).Replace("\n", string.Empty) + "...";
        }
    }
}
