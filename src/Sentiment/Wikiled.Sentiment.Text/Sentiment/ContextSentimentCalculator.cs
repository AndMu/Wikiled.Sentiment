using System;
using System.Collections.Generic;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public class ContextSentimentCalculator : IContextSentiment
    {
        private readonly IWordItemRelationships parent;

        private readonly List<SentimentValue> sentiments = new List<SentimentValue>();

        public ContextSentimentCalculator(IWordItemRelationships parent)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public IList<SentimentValue> Sentiments => sentiments.AsReadOnly();

        public void Process()
        {
            sentiments.Clear();
            if (parent.Sentiment != null)
            {
                sentiments.Add(parent.Sentiment);
            }

            List<ISentence> sentences = new List<ISentence>();
            if (parent.Part.Sentence.Previous != null)
            {
                sentences.Add(parent.Part.Sentence.Previous);
            }

            sentences.Add(parent.Part.Sentence);
            if (parent.Part.Sentence.Next != null)
            {
                sentences.Add(parent.Part.Sentence.Next);
            }

            foreach (var sentence in sentences)
            {
                foreach (var wordItem in sentence.Occurrences.GetImportant())
                {
                    if (!wordItem.IsSentiment ||
                        wordItem == parent.Owner)
                    {
                        continue;
                    }

                    var sentiment = wordItem.Relationship.Sentiment;
                    if (sentiment == null)
                    {
                        continue;
                    }

                    int distance = GetDistance(wordItem);
                    sentiment = sentiment.GetDistanced(distance);
                    sentiments.Add(sentiment);
                }
            }
        }

        private int GetDistance(IWordItem sentiment)
        {
            int parentIndex = parent.Owner.WordIndex;
            int distance = Math.Abs(sentiment.WordIndex - parentIndex);
            foreach (var relatedQuant in sentiment.Relationship.PriorQuants)
            {
                int currentDistance = Math.Abs(relatedQuant.WordIndex - parentIndex);
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                }
            }

            if (sentiment.Relationship.Inverted != null)
            {
                int currentDistance = Math.Abs(sentiment.Relationship.Inverted.WordIndex - parentIndex);
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                }
            }

            return distance;
        }
    }
}
