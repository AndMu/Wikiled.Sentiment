using System;
using System.Collections.Generic;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.TestLogic.Shared.Helpers
{
    public class TestWordItemRelationship : IWordItemRelationships
    {
        public TestWordItemRelationship()
        {
            RelatedFeatures = new List<IWordItem>();  
            Related = new List<IWordItem>();
            ContextSentiments = new List<SentimentValue>();
            PriorQuants = new List<IWordItem>();
        }

        public void Reset()
        {
        }

        public void Add(IWordItem relatedWord)
        {
        }

        public Tuple<IWordItem, double> DistanceToNearest(Func<IWordItem, bool> forward, Func<IWordItem, bool> backward)
        {
            throw new NotSupportedException();
        }

        public IList<IWordItem> RelatedFeatures { get; set; }

        public IWordItem Owner { get; set; }

        public IList<IWordItem> Related { get; set; }

        public string[] Views { get; set; }

        public IList<SentimentValue> ContextSentiments { get; }

        public SentimentValue Sentiment { get; set; }

        public ISentencePart Part { get; set; }

        public IWordItem Inverted { get; set; }

        public IEnumerable<IWordItem> PriorQuants { get; }

        public IWordItem Previous { get; set; }

        public IWordItem Next { get; set; }

        IEnumerable<IWordItem> IWordItemRelationships.Related => throw new NotImplementedException();
    }
}
