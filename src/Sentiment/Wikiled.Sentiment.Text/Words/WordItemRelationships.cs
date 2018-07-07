using System.Collections.Generic;
using System.Linq;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Data.Weighting;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Text.Words
{
    public class WordItemRelationships : IWordItemRelationships
    {
        private readonly IWordsHandler handler;

        private readonly List<IWordItem> related = new List<IWordItem>();

        private SentimentValue sentiment;

        private bool sentimentResolved;

        public WordItemRelationships(IWordsHandler handler, IWordItem parent)
        {
            this.handler = handler ?? throw new System.ArgumentNullException(nameof(handler));
            Owner = parent ?? throw new System.ArgumentNullException(nameof(parent));
            Reset();
        }

        public IWordItem Inverted
        {
            get
            {
                if (Previous != null &&
                    Previous.IsInvertor &&
                    Previous.Relationship?.Owner == Owner)
                {
                    return Previous;
                }

                IWordItem answer;
                IWordItemRelationships current = this;
                while (current.Previous != null)
                {
                    current = current.Previous.Relationship;
                    if (current.Owner.IsInvertor)
                    {
                        answer = current.Owner.GetInvertedTarget();
                        if (answer == Owner)
                        {
                            return current.Owner;
                        }

                        break;
                    }
                }

                current = this;
                while (current.Next != null)
                {
                    current = current.Next.Relationship;
                    if (current.Owner.IsInvertor)
                    {
                        answer = current.Owner.GetInvertedTarget();
                        if (answer == Owner)
                        {
                            return current.Owner;
                        }

                        break;
                    }
                }

                return null;
            }
        }

        public IEnumerable<IWordItem> PriorRelated => related.Where(item => item.WordIndex < Owner.WordIndex);

        public IEnumerable<IWordItem> AfterRelated => related.Where(item => item.WordIndex > Owner.WordIndex);

        public IWordItem Next { get; set; }

        public IWordItem Owner { get; }

        public ISentencePart Part { get; set; }

        public IWordItem Previous { get; set; }

        public IEnumerable<IWordItem> PriorQuants
        {
            get
            {
                IWordItemRelationships current = this;
                while (current.Previous != null)
                {
                    current = current.Previous.Relationship;
                    if (current.Owner.QuantValue.HasValue)
                    {
                        yield return current.Owner;
                    }

                    if (!current.Owner.IsStopWord)
                    {
                        yield break;
                    }
                }
            }
        }

        public SentimentValue Sentiment
        {
            get
            {
                if (!sentimentResolved)
                {
                    sentiment = ResolveSentiment();
                    sentimentResolved = true;
                }

                return sentiment;
            }
            set
            {
                sentiment = value;
                if (value != null)
                {
                    sentimentResolved = true;
                }
            }
        }

        public void Add(IWordItem relatedWord)
        {
            if (relatedWord == null)
            {
                throw new System.ArgumentNullException(nameof(relatedWord));
            }

            related.Add(relatedWord);
            Reset();
        }

        public void Reset()
        {
            sentimentResolved = false;
        }

        private SentimentValue ResolveSentiment()
        {
            var sentimentValue = handler?.MeasureSentiment(Owner);
            if (Owner.IsInvertor &&
                !Owner.IsUsedInSentiment())
            {
                if (handler?.DisableFeatureSentiment == true)
                {
                    return null;
                }

                // is something inverted, but unknown to us 
                if (sentimentValue != null)
                {
                    return sentimentValue;
                }

                return SentimentValue.CreateInvertor(Owner);
            }

            if (sentimentValue == null)
            {
                return null;
            }

            sentimentValue = new SentimentCalculator(sentimentValue).Calculate();
            return sentimentValue;
        }
    }
}
