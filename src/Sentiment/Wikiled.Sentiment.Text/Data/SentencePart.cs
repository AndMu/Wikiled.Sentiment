using System.Collections.Generic;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Helpers;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Data
{
    internal class SentencePart : ISentencePart
    {
        private readonly List<IWordItem> items = new List<IWordItem>();

        private readonly AutoEvictingDictionary<IWordItem, IWordItem> windowItems = new AutoEvictingDictionary<IWordItem, IWordItem>(length: 3);

        public SentencePart(ISentence sentence, ISentencePart previous)
        {
            Sentence = sentence;
            Previous = previous;
        }

        public IList<IWordItem> Occurrences => items.AsReadOnly();

        public ISentencePart Previous { get; }

        public ISentence Sentence { get; }

        public override string ToString()
        {
            return $"Part with [{Occurrences.Count}]";
        }

        public void AddItem(IWordItem item)
        {
            if (item == null)
            {
                return;
            }

            if (!item.IsSimple)
            {
                throw new ProcessingException("Can add only simple word item type");
            }

            if (item.IsSimpleConjunction())
            {
                return;
            }

            if (items.Count > 0)
            {
                var previous = items[items.Count - 1];
                item.Relationship.Previous = previous;
                previous.Relationship.Next = item;
            }

            items.Add(item);
            if (item.Parent != null)
            {
                item.Parent.Relationship.Part = this;
            }

            item.Relationship.Part = this;
            AddRelationship(item);
            windowItems.Add(item, item);
            windowItems.Increment();
        }

        private void AddRelationship(IWordItem currentItem)
        {
            foreach (var item in windowItems.Keys)
            {
                currentItem.Relationship.Add(item);
                item.Relationship.Add(currentItem);
            }
        }
    }
}
