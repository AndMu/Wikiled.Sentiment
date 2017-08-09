using System;
using System.Collections.Generic;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Data
{
    public interface IWordItemRelationships
    {
        void Reset();

        void Add(IWordItem relatedWord);

        Tuple<IWordItem, double> DistanceToNearest(Func<IWordItem, bool> forward, Func<IWordItem, bool> backward);

        IWordItem Owner { get; }

        IEnumerable<IWordItem> PriorRelated { get; }

        IEnumerable<IWordItem> AfterRelated { get; }

        SentimentValue Sentiment { get; set; }

        ISentencePart Part { get; set; }

        IWordItem Inverted { get; }

        IEnumerable<IWordItem> PriorQuants { get; }

        IWordItem Previous { get; set; }

        IWordItem Next { get; set; }
    }
}
