using System.Collections.Generic;
using System.Linq;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Extensions;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.POS.Tags;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Inquirer.Data;

namespace Wikiled.Sentiment.Text.Words
{
    public class Phrase : IPhrase
    {
        private readonly List<IWordItem> nonStop = new List<IWordItem>();

        private readonly List<IWordItem> occurrences = new List<IWordItem>();

        private Phrase(BasePOSType pos)
        {
            Guard.NotNull(() => pos, pos);
            Text = string.Empty;
            POS = pos;
            Stemmed = string.Empty;
        }

        public IEnumerable<IWordItem> AllWords => occurrences;

        public NamedEntities Entity { get; set; }

        public bool IsFeature { get; set; }

        public bool IsFixed
        {
            get
            {
                return occurrences.Any(item => item.IsFixed);
            }
        }

        public bool IsInvertor
        {
            get
            {
                return occurrences.Any(item => item.IsInvertor);
            }
        }

        public bool IsQuestion
        {
            get
            {
                return occurrences.Any(item => item.IsQuestion);
            }
        }

        public bool IsSentiment { get; set; }

        public bool IsSimple => false;

        public bool IsStopWord
        {
            get
            {
                return occurrences.Count > 0 && occurrences.All(item => item.IsStopWord);
            }
        }

        public bool IsTopAttribute { get; set; }

        public int NonStopWordCount => nonStop.Count;

        public string NormalizedEntity { get; set; }

        public IWordItem Parent { get; set; }

        public BasePOSType POS { get; }

        public double? QuantValue
        {
            get
            {
                return occurrences.Max(item => item.QuantValue);
            }
        }

        public InquirerDefinition Inquirer { get; }

        public IWordItemRelationships Relationship { get; private set; }

        public string Stemmed { get; private set; }

        public string Text { get; private set; }

        public int WordIndex { get; set; }

        public static Phrase Create(IWordsHandler wordsHandlers, BasePOSType pos)
        {
            Guard.NotNull(() => wordsHandlers, wordsHandlers);
            var item = new Phrase(pos);
            item.Relationship = new WordItemRelationships(wordsHandlers, item);
            return item;
        }

        public void Add(IWordItem word)
        {
            Guard.NotNull(() => word, word);
            if (occurrences.Contains(word) ||
                word.IsSimpleConjunction())
            {
                return;
            }

            occurrences.Add(word);
            if (!word.IsStopWord)
            {
                nonStop.Add(word);
            }

            word.Parent = this;
            Relationship.Part = word.Relationship.Part;
            if (Relationship.Previous != null)
            {
                Relationship.Previous = word.Relationship.Previous;
            }

            Relationship.Next = word.Relationship.Next;

            Stemmed = nonStop
                .Select(item => item.Stemmed)
                .AccumulateItems(" ");
            Text = occurrences
                .Select(item => item.Text)
                .AccumulateItems(" ");
            if (string.IsNullOrEmpty(Stemmed))
            {
                Stemmed = Text;
            }
        }

        public void Reset()
        {
            Parent?.Reset();
            Relationship?.Reset();
        }

        public override string ToString()
        {
            return $"Phrase: {Text} [{POS}]";
        }
    }
}
