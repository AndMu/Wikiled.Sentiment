using System;

namespace Wikiled.Sentiment.Text.Persitency
{
    public interface IIndexRegistry
    {
        DateTime Date { get; set; }

        string Tag { get; set; }

        string File { get; set; }
    }
}