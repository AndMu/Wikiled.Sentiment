using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Extensions;

namespace Wikiled.Sentiment.Text.Persitency
{
    public class IndexRegistryHandler<T>
        where T : class, IIndexRegistry 
    {
        private readonly ConcurrentDictionary<string, List<T>> table = new ConcurrentDictionary<string, List<T>>(StringComparer.OrdinalIgnoreCase);

        private readonly ConcurrentDictionary<string, T> tableIndex = new ConcurrentDictionary<string, T>(StringComparer.OrdinalIgnoreCase);

        private readonly ConcurrentDictionary<DateTime, ConcurrentBag<T>> tableByDate = new ConcurrentDictionary<DateTime, ConcurrentBag<T>>();

        public bool Add(T index)
        {
            Guard.NotNull(() => index, index);
            string key = GenerateKey(index);
            if (tableIndex.ContainsKey(key))
            {
                return false;
            }

            tableIndex[key] = index;
            table.GetSafeCreate(index.Tag).Add(index);
            var list = tableByDate.GetSafeCreate(index.Date.Date);
            list.Add(index);
            return true;
        }

        public IEnumerable<T> Find(string tag)
        {
            Guard.NotNullOrEmpty(() => tag, tag);
            List<T> registries;
            if (table.TryGetValue(tag, out registries))
            {
                return registries;
            }

            return new T[] { };
        }

        public IEnumerable<T> FindByDate(DateTime date)
        {
            ConcurrentBag<T> registries;
            if (tableByDate.TryGetValue(date.Date, out registries))
            {
                return registries;
            }

            return new T[] { };
        }

        private string GenerateKey(T item)
        {
            return string.Format("{0}:{1}:{2}", item.Date, item.Tag, item.File);
        }

        public IEnumerable<string> IndexKeys
        {
            get { return table.Keys; }
        }

        [XmlArray("Records"), XmlArrayItem("Record")]
        public T[] Records
        {
            get { return tableIndex.Values.ToArray(); }
            set
            {
                table.Clear();
                tableIndex.Clear();
                tableByDate.Clear();
                if (value == null)
                {
                    return;
                }

                foreach (var article in value)
                {
                    Add(article);
                }
            }
        }
    }
}
