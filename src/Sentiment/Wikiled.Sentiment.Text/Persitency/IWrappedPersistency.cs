using System;
using System.Collections.Generic;

namespace Wikiled.Sentiment.Text.Persitency
{
    public interface IWrappedPersistency<T> : IIndexedPersistency<WrappedPersistencyItem<T>, IndexRegistry>
        where T : class
    {
        void Save(T result);

        IEnumerable<string> GetAllIndexes();

        IEnumerable<Lazy<T>> GetAllResult();

        IEnumerable<Lazy<T>> FindResult(Func<IndexRegistry, bool> filter);

        bool Contains(Func<IndexRegistry, bool> filter);

        bool Contains(string index, DateTime date);
    }
}