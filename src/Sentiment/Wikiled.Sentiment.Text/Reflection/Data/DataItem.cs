using Wikiled.Core.Utility.Arguments;

namespace Wikiled.Sentiment.Text.Reflection.Data
{
    public class DataItem : IDataItem
    {
        public DataItem(string category, string name, string description, object value)
        {
            Guard.NotNullOrEmpty(() => name, name);
            Category = category;
            Value = value;
            Name = name;
            Description = description;
        }

        public string Category { get; }

        public string Description { get; }

        public string FullName => Category + " " + Name;

        public string Name { get; }

        public object Value { get; set; }
    }
}
