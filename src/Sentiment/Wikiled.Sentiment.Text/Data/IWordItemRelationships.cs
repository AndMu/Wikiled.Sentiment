using System.Collections.Generic;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Data
{
    public interface IWordItemRelationships
    {
        IEnumerable<IWordItem> Related { get; }

        string[] Views { get; }

        IWordItem Inverted { get; }

        IWordItem Next { get; set; }

        IWordItem Owner { get; }

        ISentencePart Part { get; set; }

        IWordItem Previous { get; set; }

        IEnumerable<IWordItem> PriorQuants { get; }

        SentimentValue Sentiment { get; set; }

        void Add(IWordItem relatedWord);

        void Reset();
    }
}
