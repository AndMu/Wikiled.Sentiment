using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Aspects
{
    public class WordOccurenceTracker
    {
        private readonly ConcurrentDictionary<string, IWordItem> table = new ConcurrentDictionary<string, IWordItem>(StringComparer.OrdinalIgnoreCase);

        private readonly ConcurrentDictionary<string, ConcurrentBag<IPhrase>> tablePhrase = new ConcurrentDictionary<string, ConcurrentBag<IPhrase>>(StringComparer.OrdinalIgnoreCase);

        private int total;

        public int Total => total;

        public IEnumerable<IWordItem> Words => table.Values;

        public IEnumerable<IPhrase> GetPhrases(int cutOff)
        {
            return tablePhrase.Values
                              .Where(item => item.Count >= cutOff)
                              .OrderByDescending(item => item.Count)
                              .Select(item => item.First()).Take(10);
        }

        public void AddWord(IWordItem wordItem)
        {
            Guard.NotNull(() => wordItem, wordItem);
            Interlocked.Increment(ref total);
            table[wordItem.Text] = wordItem;
        }

        public void AddPhrase(IPhrase phrase)
        {
            Guard.NotNull(() => phrase, phrase);
            if (!tablePhrase.TryGetValue(phrase.Text, out ConcurrentBag<IPhrase> list))
            {
                list = new ConcurrentBag<IPhrase>();
                if (!tablePhrase.TryAdd(phrase.Text, list))
                {
                    if (!tablePhrase.TryGetValue(phrase.Text, out list))
                    {
                        list = new ConcurrentBag<IPhrase>();
                    }
                }

                tablePhrase[phrase.Text] = list;
            }

            list.Add(phrase);
        }
    }
}
