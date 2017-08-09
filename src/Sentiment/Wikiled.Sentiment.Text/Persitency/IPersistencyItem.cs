using System;

namespace Wikiled.Sentiment.Text.Persitency
{
    public interface IPersistencyItem<out T>
        where T : class, IIndexRegistry
    {
        T GenerateIndex(string file);

        DateTime Date { get; }

        string Type { get; }

        string Tag { get; }
    }
}
