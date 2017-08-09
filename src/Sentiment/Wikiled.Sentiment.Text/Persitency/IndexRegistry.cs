using System;

namespace Wikiled.Sentiment.Text.Persitency
{
    public class IndexRegistry : IIndexRegistry
    {
        public DateTime Date { get; set; }

        public string Tag { get; set; }

        public string File { get; set; }
    }
}
