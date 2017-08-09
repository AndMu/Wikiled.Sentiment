using System;

namespace Wikiled.Sentiment.Text.Persitency
{
    public class SimplePersistencyItem : IPersistencyItem<IndexRegistry>
    {
        public IndexRegistry GenerateIndex(string file)
        {
            return new IndexRegistry
                       {
                           Tag = Type,
                           Date = Date,
                           File = file
                       };
        }

        public DateTime Date { get; set; }

        public string Type { get; set; }

        public string Tag { get; set; }
    }
}
