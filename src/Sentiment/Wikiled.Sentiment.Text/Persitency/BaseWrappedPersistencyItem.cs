using System;
using System.Xml.Serialization;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.Sentiment.Text.Persitency
{
    [XmlRoot("DataItem")]
    public abstract class BaseWrappedPersistencyItem<T, TI> : IPersistencyItem<TI>
        where TI : class, IIndexRegistry
    {
        protected BaseWrappedPersistencyItem() { }

        protected BaseWrappedPersistencyItem(DateTime date, string type, string tag, T instance)
        {
            Guard.NotNullOrEmpty(() => type, type);
            Guard.NotNullOrEmpty(() => tag, tag);
            Date = date;
            Type = type;
            Tag = tag;
            Instance = instance;
        }

        public T Instance { get; set; }

        public abstract TI GenerateIndex(string file);

        public DateTime Date { get; set; }

        public string Type { get; set; }

        public string Tag { get; set; }
    }
}
