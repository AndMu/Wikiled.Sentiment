using System;
using System.Xml.Serialization;

namespace Wikiled.Sentiment.Text.Persitency
{
    [XmlRoot("DataItem")]
    public class WrappedPersistencyItem<T> : BaseWrappedPersistencyItem<T, IndexRegistry>
    {
        public WrappedPersistencyItem() { }

        public WrappedPersistencyItem(DateTime date, string type, string tag, T instance)
            : base(date, type, tag, instance)
        {
        }

        public override IndexRegistry GenerateIndex(string file)
        {
            return new IndexRegistry
            {
                Tag = Type,
                Date = Date,
                File = file
            };
        }
    }
}
