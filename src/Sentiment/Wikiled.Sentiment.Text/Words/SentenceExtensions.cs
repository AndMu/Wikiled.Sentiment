using System.Collections.Generic;
using System.Linq;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Text.Words
{
    public static class SentenceExtensions
    {
        public static IEnumerable<Phrase> GetPhrases(this IEnumerable<IWordItem> words)
        {
            Phrase parent = null;
            foreach(var wordItem in words)
            {
                var currentParent = wordItem.Parent as Phrase;
                if(currentParent == null)
                {
                    continue;
                }

                if(parent == currentParent)
                {
                    continue;
                }

                parent = currentParent;
                yield return parent;
            }
        }

        public static IEnumerable<IWordItem> GetImportant(this IEnumerable<IWordItem> words)
        {
            IPhrase parent = null;
            foreach (var wordItem in words)
            {
                if (wordItem.IsStopWord ||
                    wordItem.IsConjunction())
                {
                    continue;
                }

                if (!(wordItem.Parent is IPhrase currentParent) ||
                    currentParent.AllWords.Count() <= 1)
                {
                    yield return wordItem;
                    continue;
                }

                if (parent == currentParent)
                {
                    continue;
                }

                parent = currentParent;
                if (parent.IsFeature || parent.IsSentiment)
                {
                    yield return parent;
                }
                else
                {
                    foreach (var child in parent.AllWords)
                    {
                        if (child.IsStopWord &&
                            !WordTypeResolver.Instance.IsSpecialEndSymbol(child.Text))
                        {
                            continue;
                        }

                        yield return child;
                    }
                }
            }
        }

        public static RatingData CalculateRating(this ISentence sentence)
        {
            var value = RatingData.Accumulate(sentence.Parts.Select(item => item.CalculateRating()));
            return value;
        }

        public static RatingData CalculateRating(this ISentencePart sentence)
        {
            var data = new RatingData();
            if (sentence.Occurrences.Count == 0)
            {
                return data;
            }

            foreach (var wordItem in sentence.Occurrences.GetImportant())
            {
                if (wordItem.Relationship.Sentiment == null)
                {
                    continue;
                }

                data.AddSentiment(wordItem.Relationship.Sentiment.DataValue);
            }

            if (!WordTypeResolver.Instance.IsInvertingConjunction(sentence.Occurrences[0].Text))
            {
                return data;
            }

            if (!data.HasValue)
            {
                // make oposite to previous part and twice lower
                var previous = sentence.Previous?.CalculateRating().RawRating;
                if (previous != null)
                {
                    data.AddSentiment(new SentimentValueData(-previous.Value / 2));
                }
            }

            return data;
        }
    }
}
