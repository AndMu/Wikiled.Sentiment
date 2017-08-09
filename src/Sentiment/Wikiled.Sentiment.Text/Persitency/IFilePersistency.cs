using System;
using System.Collections.Generic;
using System.IO;

namespace Wikiled.Sentiment.Text.Persitency
{
    public interface IFilePersistency<T>
        where T : class
    {
        event EventHandler<LoadingEventArgs> Loading;

        event EventHandler<PersitencyEventArgs<T>> Saved;

        event EventHandler<PersitencyEventArgs<T>> Loaded;

        IEnumerable<T> LoadAll(DirectoryInfo directory);

        T LoadItem(FileInfo file);

        FileInfo Save(T record, string file);

        bool UseCompression { get; }
    }
}