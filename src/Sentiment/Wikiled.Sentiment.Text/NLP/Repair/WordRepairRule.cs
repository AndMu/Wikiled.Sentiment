using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.NLP.Repair
{
    public class WordRepairRule
    {
        public WordRepairRule()
        {
            SuccesfulResult = true;
        }

        public string Word { get; set; }

        public string Lemma { get; set; }

        public WordItemType Type { get; set; }

        public bool SuccesfulResult { get; set; }

        public RuleSet[] Set { get; set; }
    }
}
