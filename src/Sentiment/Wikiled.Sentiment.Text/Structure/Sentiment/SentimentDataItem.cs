using Wikiled.Core.Utility.Arguments;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Structure.Sentiment
{
    public class SentimentDataItem
    {
        public SentimentDataItem(int index, string text, double? value, SentimentLevel level)
        {
            Guard.IsValid(() => index, index, item => item >= 0, "index");
            Guard.NotNullOrEmpty(() => text, text);
            Index = index;
            Text = text;
            Value = value;
            Level = level;
            Border = 1;
        }

        public int Border { get; set; }

        public int Index { get; }

        public bool IsAnomaly { get; set; }

        public bool IsSelected { get; set; }

        public SentimentLevel Level { get; }

        public SentenceItem ParentSentence { get; set; }

        public WordEx ParentWord { get; set; }

        public string Text { get; }

        public double? Value { get; }
    }
}
