using System;
using System.Xml.Serialization;
using Wikiled.Core.Utility.Serialization;
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

        public void InitDocument(IParsedReview review)
        {
            Review = review;
            Document = Review.GenerateDocument(NullRatingAdjustment.Instance);
            Document.Stars = Stars;
        }

        public void InitDocument(IWordsExtraction extraction)
        {
            Document = extraction.GetDocument(Text.SanitizeXmlString());
        }

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

        [XmlIgnore]
        public string OriginalText { get; }

        [XmlIgnore]
        public IParsedReview Review { get; private set; }

        [XmlAttribute]
        public DateTime Date { get; set; }

        [XmlElement]
        public Document Document { get; set; }

        public string[] Other { get; set; }

        public string Text { get; set; }

        public double Stars { get; set; }
    }
}
