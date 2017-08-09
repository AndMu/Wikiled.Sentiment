using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Wikiled.Sentiment.Text.Persitency
{
    public class MultiThroughFilePersistencyHandler<T, TI> : MultiFilePersistencyHandler<T, TI>
        where TI : class, IIndexRegistry
        where T : class, IPersistencyItem<TI>
    {
        public MultiThroughFilePersistencyHandler(string name, string path, bool compress = true)
            : base(name, path, compress)
        {
        }

        public MultiThroughFilePersistencyHandler(string path, bool compress = true)
            : base(path, compress)
        {
        }

        public override void Add(params T[] records)
        {
            foreach (var record in records)
            {
                Save(record);
            }
        }

        public override List<T> RetrieveAll(string index)
        {
            string path = Path.Combine(PersistencyPath.FullName, index);
            var directory = new DirectoryInfo(path);
            if (!directory.Exists)
            {
                return null;
            }

            return Persistency.LoadAll(directory).ToList();
        }

        protected override List<T> RetrieveItems(string index, DateTime date, int max)
        {
            return RetrieveAll(index);
        }

        protected override void RegisterRecord(T record)
        {
        }
    }
}
