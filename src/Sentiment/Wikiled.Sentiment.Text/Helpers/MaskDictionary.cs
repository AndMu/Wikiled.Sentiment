using System.Collections.Generic;

namespace Wikiled.Sentiment.Text.Helpers
{
    public class MaskDictionary<T>
    {
        private readonly Dictionary<string, T> innerTable;

        private int minimum = int.MaxValue;

        public MaskDictionary(IEqualityComparer<string> equalityComparer = null)
        {
            innerTable = new Dictionary<string, T>(equalityComparer);
        }

        public int Count => innerTable.Count;

        public T this[string key]
        {
            get
            {
                if (key.Length > minimum ||
                    !innerTable.ContainsKey(key))
                {
                    return this[key.Substring(0, key.Length - 1)];
                }

                return innerTable[key];
            }
            set
            {
                CheckLength(key);
                innerTable[key] = value;
            }
        }

        public void Add(string key, T value)
        {
            CheckLength(key);
            innerTable.Add(key, value);
        }

        public void Clear()
        {
            innerTable.Clear();
        }

        public bool ContainsKey(string key)
        {
            if (key.Length < minimum ||
                key.Length == 0)
            {
                return false;
            }

            return innerTable.ContainsKey(key) || ContainsKey(key.Substring(0, key.Length - 1));
        }

        public string FindKey(string key)
        {
            if (key.Length < minimum ||
                key.Length == 0)
            {
                return null;
            }

            return innerTable.TryGetValue(key, out T value) ? key : FindKey(key.Substring(0, key.Length - 1));
        }

        public Dictionary<string, T>.Enumerator GetEnumerator()
        {
            return innerTable.GetEnumerator();
        }

        public bool Remove(string key)
        {
            if (key.Length == minimum)
            {
                minimum = int.MaxValue;
                foreach (var existing in innerTable)
                {
                    CheckLength(existing.Key);
                }
            }

            return innerTable.Remove(key);
        }

        public bool TryGetValue(string key, out T value)
        {
            value = default(T);
            if (key.Length < minimum ||
                key.Length == 0)
            {
                return false;
            }

            return innerTable.TryGetValue(key, out value) || TryGetValue(key.Substring(0, key.Length - 1), out value);
        }

        private void CheckLength(string key)
        {
            minimum = key.Length < minimum ? key.Length : minimum;
        }
    }
}
