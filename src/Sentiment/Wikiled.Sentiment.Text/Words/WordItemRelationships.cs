using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Data.Weighting;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Text.Analysis.POS;

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
            Guard.NotNull(() => parent, parent);
            this.handler = handler;
            Owner = parent;
            Reset();
        }

        public IWordItem Inverted
        {
            get
            {
                if (Previous != null &&
                    Previous.IsInvertor)
                {
                    return Previous;
                }

                Tuple<IWordItem, double> answer;
                IWordItemRelationships current = this;
                while (current.Previous != null)
                {
                    current = current.Previous.Relationship;
                    if (current.Owner.IsInvertor)
                    {
                        answer = current.Owner.GetInvertedTarget();
                        if (answer != null &&
                            answer.Item1 == Owner)
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
                        if (answer != null &&
                            answer.Item1 == Owner)
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
            Guard.NotNull(() => relatedWord, relatedWord);
            related.Add(relatedWord);
            Reset();
        }

        public Tuple<IWordItem, double> DistanceToNearest(Func<IWordItem, bool> forward, Func<IWordItem, bool> backward)
        {
            var max = 2;
            IWordItemRelationships current = this;
            double i = 0;
            double weightSentiment = 0.9;
            double weightFeature = 2;
            double weight = 1;
            var stopWeight = 0.5;
            Tuple<IWordItem, double> found = null;
            while (current.Next != null)
            {
                current = current.Next.Relationship;
                if (current.Owner.POS.WordType == WordType.SeparationSymbol)
                {
                    break;
                }

                if (current.Owner.IsStopWord)
                {
                    i += stopWeight;
                    continue;
                }

                i += weight;
                if (i >= max)
                {
                    break;
                }

                var coef = current.Owner.IsFeature ? weightFeature : weightSentiment;
                var value = i * coef;
                if (forward(current.Owner))
                {
                    if (found == null ||
                        found.Item2 > value)
                    {
                        found = new Tuple<IWordItem, double>(current.Owner, value);
                    }
                }
            }

            max = 3;
            i = 0;
            weight = 2;
            current = this;
            while (current.Previous != null)
            {
                current = current.Previous.Relationship;
                if (current.Owner.POS.WordType == WordType.SeparationSymbol)
                {
                    break;
                }

                if (current.Owner.IsStopWord)
                {
                    i += stopWeight;
                    continue;
                }

                i += weight;
                if (i >= max)
                {
                    break;
                }

                if (found != null && found.Item2 <= i)
                {
                    break;
                }

                if (backward(current.Owner))
                {
                    return new Tuple<IWordItem, double>(current.Owner, i);
                }
            }

            return found;
        }

        public void Reset()
        {
            sentimentResolved = false;
        }

        private SentimentValue ResolveSentiment()
        {
            if (Owner.IsStopWord)
            {
                return null;
            }

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
