using System.Collections.Generic;
using System.Linq;
using Wikiled.Common.Extensions;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;
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
            Text = string.Empty;
            POS = pos ?? throw new System.ArgumentNullException(nameof(pos));
            Stemmed = string.Empty;
        }

        public IEnumerable<IWordItem> AllWords => occurrences;

        public ISessionContext Session { get; private set; }

        public NamedEntities Entity { get; set; }

        public string CustomEntity { get; set; }

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

        public static Phrase Create(IContextWordsHandler wordsHandlers, BasePOSType pos)
        {
            if (wordsHandlers == null)
            {
                throw new System.ArgumentNullException(nameof(wordsHandlers));
            }

            var item = new Phrase(pos);
            item.Relationship = new WordItemRelationships(wordsHandlers, item);
            item.Session = wordsHandlers.Context;
            return item;
        }

        public void Add(IWordItem word)
        {
            if (word == null)
            {
                throw new System.ArgumentNullException(nameof(word));
            }

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
