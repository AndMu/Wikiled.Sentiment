using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Text.NLP.Repair
{
    public class Rule
    {
        public int Index { get; set; }

        public WordType NextWordPOS { get; set; }

        public string Ending { get; set; }

        public string Word { get; set; }
    }
}
