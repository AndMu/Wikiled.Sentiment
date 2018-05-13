using System;
using System.Collections.Generic;
using System.Linq;

namespace Wikiled.Sentiment.Text.Helpers
{
    public class AutoEvictingDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, int> queue;

        private readonly int queueLength;

        private readonly Dictionary<TKey, int> totalTable;

        private readonly Dictionary<TKey, TValue> values;

        public AutoEvictingDictionary(IEqualityComparer<TKey> comparer = null, int length = 4)
        {
            queueLength = length;
            values = new Dictionary<TKey, TValue>(comparer);
            queue = new Dictionary<TKey, int>(comparer);
            totalTable = new Dictionary<TKey, int>(comparer);
        }

        public int Count => values.Count;

        public TKey[] Keys => values.Keys.ToArray();

        public TValue[] Values => values.Values.ToArray();

        public TValue this[TKey key]
        {
            get => Get(key);
            set => Add(key, value);
        }

        public virtual void Add(TKey key, TValue value)
        {
            values[key] = value;
            queue[key] = 1;
            Increment(key, totalTable);
        }

        public virtual bool ContainsKey(TKey key)
        {
            return values.ContainsKey(key);
        }

        public virtual void Touch(TKey key)
        {
            if (!ContainsKey(key))
            {
                throw new ArgumentOutOfRangeException("key", "Can't touch unknown item");
            }

            Add(key, Get(key));
        }

        public void Clear()
        {
            totalTable.Clear();
            queue.Clear();
            values.Clear();
        }

        public TValue Get(TKey key)
        {
            return values.TryGetValue(key, out TValue value) ? value : default(TValue);
        }

        public void Increment()
        {
            foreach (var item in queue.ToArray())
            {
                if (item.Value >= queueLength)
                {
                    Remove(item.Key);
                }
                else
                {
                    queue[item.Key] = item.Value + 1;
                }
            }
        }

        public void Remove(TKey key)
        {
            queue.Remove(key);
            values.Remove(key);
        }

        public int TotalOccurences(TKey key)
        {
            return !totalTable.TryGetValue(key, out int total) ? 0 : total;
        }

        protected static void Increment(TKey key, Dictionary<TKey, int> table)
        {
            if (!table.TryGetValue(key, out int total))
            {
                total = 0;
            }

            total++;
            table[key] = total;
        }
    }
}
