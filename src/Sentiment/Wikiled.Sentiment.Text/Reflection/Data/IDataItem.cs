namespace Wikiled.Sentiment.Text.Reflection.Data
{
    public interface IDataItem
    {
        string FullName { get; }

        string Name { get; }

        string Description { get; }

        object Value { get; set; }
    }
}
