using System.Collections.Generic;
using System.Linq;
using Wikiled.MachineLearning.Mathematics;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Text.Analysis.Words;

namespace Wikiled.Sentiment.Text.Words
{
    public static class SentenceExtensions
    {
        public static IEnumerable<Phrase> GetPhrases(this IEnumerable<IWordItem> words)
        {
            Phrase parent = null;
            foreach(var wordItem in words)
            {
                if (!(wordItem.Parent is Phrase currentParent))
                {
                    continue;
                }

                if (parent == currentParent)
                {
                    continue;
                }

                parent = currentParent;
                yield return parent;
            }
        }

        public static IEnumerable<IWordItem> GetImportant(this IEnumerable<IWordItem> words)
        {
            return GetImportantInternal(words).Distinct();
        }


        public static RatingData CalculateRating(this ISentence sentence)
        {
            var data = new RatingData();
            var important = sentence.Occurrences.GetImportant();
            IWordItem firstWord = null;
            foreach (var wordItem in important)
            {
                if (firstWord == null)
                {
                    firstWord = wordItem;
                }

                if (wordItem.Relationship.Sentiment == null)
                {
                    continue;
                }

                data.AddSentiment(wordItem.Relationship.Sentiment.DataValue);
            }

            if (firstWord == null)
            {
                return data;
            }

            if (!WordTypeResolver.Instance.IsInvertingConjunction(firstWord.Text))
            {
                return data;
            }

            if (!data.HasValue)
            {
                // make opposite to previous part and twice lower
                var previous = sentence.Previous?.CalculateRating().RawRating;
                if (previous != null)
                {
                    data.AddSentiment(new SentimentValueData(-previous.Value / 2));
                }
            }

            return data;
        }

        private static IEnumerable<IWordItem> GetImportantInternal(this IEnumerable<IWordItem> words)
        {
            IPhrase parent = null;
            foreach (var wordItem in words)
            {
                if (wordItem.CanNotBeAttribute() &&
                    wordItem.CanNotBeFeature() &&
                    !wordItem.IsFeature &&
                    !wordItem.IsSentiment &&
                    wordItem.Relationship.Sentiment == null &&
                    !wordItem.IsInvertor)
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
                        if (child.IsFeature || child.IsSentiment)
                        {
                            yield return child;
                            continue;
                        }

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
    }
}
