using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Text.Extensions
{
    public static class WordItemRelationshipsExtension
    {
        public static IWordItem GetNextByIndex(this IWordItemRelationships current, int index)
        {
            IWordItem currentItem = current.Owner;
            while (index != 0)
            {
                if (currentItem == null)
                {
                    return null;
                }

                currentItem = index < 0 ? currentItem.Relationship.Previous : currentItem.Relationship.Next;
                index += index < 0 ? 1 : -1;
            }

            return currentItem;
        }

        public static IWordItem GetNext(this IEnumerable<(IWordItem Word, double)> words)
        {
            var grouped = from item in words
                          select 
                              new
                              {
                                  Index = item.Item2,
                                  Weight = item.Word.IsSentiment ? 3 : item.Word.IsFeature ? 2 : 1,
                                  Word = item.Word
                              };
            return grouped.OrderByDescending(item => item.Weight)
                          .ThenBy(item => item.Index)
                          .Select(item => item.Word)
                          .FirstOrDefault();
        }

        public static IEnumerable<(IWordItem Word, double)> GetNeighbours(this IWordItemRelationships current, bool forward, int max = 3)
        {
            int i = 0;
            Func<IWordItemRelationships, IWordItemRelationships> next = forward ? (Func<IWordItemRelationships, IWordItemRelationships>)(item => item.Next?.Relationship) : item => item.Previous?.Relationship;
            while (next(current) != null)
            {
                i++;
                current = next(current);
                if (current.Owner.POS.WordType == WordType.SeparationSymbol)
                {
                    yield break;
                }

                if (current.Owner.IsStopWord)
                {
                    continue;
                }

                yield return (current.Owner, i);
                if (i >= max)
                {
                    yield break;
                }
            }
        }
    }
}
