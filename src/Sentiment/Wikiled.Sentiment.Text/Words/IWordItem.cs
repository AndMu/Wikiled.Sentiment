using Wikiled.Sentiment.Text.Data;
using Wikiled.Text.Analysis.POS.Tags;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Inquirer.Data;

namespace Wikiled.Sentiment.Text.Words
{
    public interface IWordItem : ITextItem
    {
        NamedEntities Entity { get; set; }

        string CustomEntity{ get; set; }

        bool IsFeature { get; }

        bool IsFixed { get; }

        bool IsInvertor { get; }

        bool IsQuestion { get; }

        bool IsSentiment { get; }

        bool IsSimple { get; }

        bool IsStopWord { get; }

        bool IsTopAttribute { get; }

        string NormalizedEntity { get; set; }

        BasePOSType POS { get; }

        IWordItem Parent { get; set; }

        double? QuantValue { get; }

        InquirerDefinition Inquirer { get; }

        IWordItemRelationships Relationship { get; }

        int WordIndex { get; set; }

        void Reset();
    }
}