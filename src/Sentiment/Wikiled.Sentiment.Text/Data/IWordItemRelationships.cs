using System;
using System.Collections.Generic;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Data
{
    public interface IWordItemRelationships
    {
        IEnumerable<IWordItem> AfterRelated { get; }

        IWordItem Inverted { get; }

        IWordItem Next { get; set; }

        IWordItem Owner { get; }

        ISentencePart Part { get; set; }

        IWordItem Previous { get; set; }

        IEnumerable<IWordItem> PriorQuants { get; }

        IEnumerable<IWordItem> PriorRelated { get; }

        SentimentValue Sentiment { get; set; }

        void Add(IWordItem relatedWord);

        Tuple<IWordItem, double> DistanceToNearest(Func<IWordItem, bool> forward, Func<IWordItem, bool> backward);

        void Reset();
    }
}
