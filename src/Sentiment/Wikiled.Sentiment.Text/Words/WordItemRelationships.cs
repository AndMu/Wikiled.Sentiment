using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Common.Extensions;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Data.Weighting;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Words
{
    public class WordItemRelationships : IWordItemRelationships
    {
        private readonly IContextWordsHandler handler;

        private readonly List<IWordItem> related = new List<IWordItem>();

        private SentimentValue sentiment;

        private bool sentimentResolved;

        private Lazy<string[]> ngrams;

        public WordItemRelationships(IContextWordsHandler handler, IWordItem parent)
        {
            this.handler = handler;
            Owner = parent ?? throw new ArgumentNullException(nameof(parent));
            Reset();
        }

        public string[] Views => ngrams.Value;

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

        public IEnumerable<IWordItem> Related => related;

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
            // Longer words have priority
            ngrams = new Lazy<string[]>(() => GetPossibleText().OrderByDescending(item => item.Length).ToArray());
        }

        private SentimentValue ResolveSentiment()
        {
            var sentimentValue = handler?.MeasureSentiment(Owner);
            if (Owner.IsInvertor &&
                !Owner.IsUsedInSentiment())
            {
                if (handler?.Context.DisableFeatureSentiment == true)
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

        private IEnumerable<string> GetPossibleText()
        {
            yield return Owner.Text;

            if (Owner is IPhrase)
            {
                yield break;
            }

            if (!string.IsNullOrEmpty(Owner.Stemmed) &&
                Owner.Stemmed != Owner.Text)
            {
                yield return Owner.Stemmed;
            }

            if (Owner.Entity == NamedEntities.Hashtag &&
                Owner.Text.Length > 1)
            {
                yield return Owner.Text.Substring(1);
            }

            if (Owner.Session.NGram <= 1)
            {
                yield break;
            }

            for (int i = 2; i <= Owner.Session.NGram; i++)
            {
                // to avoid multiple matches we take only from current word as start point
                var list = Owner.Relationship.Related.Append(Owner)
                                .OrderBy(item => item.WordIndex)
                                .Where(item => item.WordIndex >= Owner.WordIndex && item.WordIndex < (Owner.WordIndex + i));
                var groups = MoreLinq.MoreEnumerable.Window(list, i);
                bool generated = false;
                foreach (var group in groups)
                {
                    generated = true;
                    yield return @group.Select(item => item.Text).AccumulateItems(" ");
                }

                if (!generated)
                {
                    break;
                }
            }
        }
    }
}
