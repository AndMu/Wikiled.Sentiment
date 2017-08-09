using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.Sentiment.Text.Persitency
{
    public abstract class WrappedPersistency<T> : IndexedPersistency<WrappedPersistencyItem<T>, IndexRegistry>, IWrappedPersistency<T>
        where T : class
    {
        protected WrappedPersistency(string name, string path)
            : base(name, path)
        {
        }

        protected WrappedPersistency(string path)
            : base(path)
        {
        }

        protected override IEnumerable<IndexRegistry> OnPersistencySaved(string file, WrappedPersistencyItem<T> record)
        {
            yield return new IndexRegistry
                {
                    Tag = record.Tag,
                    Date = record.Date,
                    File = file
                };
        }

        public bool Contains(string index, DateTime date)
        {
            Guard.NotNullOrEmpty(() => index, index);
            var registry = Handler.Find(index);
            return registry.Any(item => item.Date.Date == date.Date.Date);
        }

        public void Save(T result)
        {
            Guard.NotNull(() => result, result);
            Save(ConstructItem(result));
        }

        public IEnumerable<string> GetAllIndexes()
        {
            return Handler.Records.Select(item => item.Tag).Distinct();
        }

        public IEnumerable<Lazy<T>> GetAllResult()
        {
            return GetAllItems().Select(x => new Lazy<T>(() => x.Value.Instance));
        }

        public IEnumerable<Lazy<T>> FindResult(Func<IndexRegistry, bool> filter)
        {
            return GetItemsInternal(() => Handler.Records.Where(filter))
                .Select(item => new Lazy<T>(() => item.Value.Instance));
        }

        public bool Contains(Func<IndexRegistry, bool> filter)
        {
            return FindResult(filter).Any();
        }

        public Lazy<T> FindResult(string index, DateTime date)
        {
            return GetItems(index).Where(item => item.Value.Date.Date == date.Date).Select(x => new Lazy<T>(() => x.Value.Instance)).FirstOrDefault();
        }

        public IEnumerable<Lazy<T>> FindResult(string index)
        {
            return GetItems(index).Select(x => new Lazy<T>(() => x.Value.Instance));
        }

        protected abstract WrappedPersistencyItem<T> ConstructItem(T result);
    }
}
