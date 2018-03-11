using System;
using System.Xml.Serialization;
using Wikiled.Common.Serialization;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Text.Analysis.Structure;

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
            OriginalText = text;
            Text = string.IsNullOrEmpty(text) ? "<Empty>" : text.Trim();
        }

        public SingleProcessingData(Document document)
            : this(document.Text)
        {
            Document = document;
            if (document.DocumentTime.HasValue)
            {
                Date = document.DocumentTime.Value;
            }
        }

        [XmlAttribute]
        public DateTime Date { get; set; }

        [XmlElement]
        public Document Document { get; set; }

        [XmlIgnore]
        public string OriginalText { get; }

        public string[] Other { get; set; }

        [XmlIgnore]
        public IParsedReview Review { get; private set; }

        public double Stars { get; set; }

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

        public void InitDocument(IParsedReview review)
        {
            Review = review;
            Document = Review.GenerateDocument(new NullRatingAdjustment());
            Document.Stars = Stars;
        }

        public void InitDocument(IWordsExtraction extraction)
        {
            Document = extraction.GetDocument(Text.SanitizeXmlString());
        }
    }
}
