using System;

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
            Text = text?.Trim();
        }

        public DateTime? Date { get; set; }

        public double? Stars { get; set; }

        public string Author { get; set; }

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
