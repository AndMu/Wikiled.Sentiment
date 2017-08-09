using System;
using System.Collections.Generic;

namespace Wikiled.Sentiment.Text.Persitency
{
    public interface IIndexedPersistency<T, TI> : IDisposable
        where TI : class, IIndexRegistry 
        where T : class, IPersistencyItem<TI>
    {
        TI[] Records { get; }
        void LoadHeader();
        void Save(T item);
        IEnumerable<Lazy<T>> GetItems(string query);
    }
}