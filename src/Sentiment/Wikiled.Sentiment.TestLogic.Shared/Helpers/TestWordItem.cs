using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.POS.Tags;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Inquirer.Data;

namespace Wikiled.Sentiment.TestLogic.Shared.Helpers
{
    public class TestWordItem : IWordItem
    {
        public TestWordItem()
        {
            var relationship = new TestWordItemRelationship();
            Relationship = relationship;
            relationship.Owner = this;
            POS = POSTags.Instance.NN;
            IsSimple = true;
        }

        public string Text { get; set; }

        public string Stemmed { get; set; }

        public BasePOSType POS { get; set; }

        public bool IsFixed { get; set; }

        public bool IsSentiment { get; set; }

        public bool IsTopAttribute { get; set; }

        public string CustomEntity { get; set; }

        public bool IsFeature { get; set; }

        public bool IsInvertor { get; set; }
        
        public bool IsStopWord { get; set; }

        public bool IsQuestion { get; set; }

        public bool IsSimple { get; protected set; }

        public double? QuantValue { get; set; }

        public InquirerDefinition Inquirer { get; set; }

        public IWordItemRelationships Relationship { get; set; }

        public int WordIndex { get; set; }

        public void Reset()
        {
        }

        public NamedEntities Entity { get; set; }

        public string NormalizedEntity { get; set; }

        public IWordItem Parent { get; set; }
    }
}
