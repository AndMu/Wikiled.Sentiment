namespace Wikiled.Sentiment.Text.Resources
{
    public interface IConfigurationHandler
    {
        void SetConfiguration(string name, string value);

        string GetConfiguration(string name);

        T SafeGetConfiguration<T>(string name, T defaultValue);

        bool TryGetConfiguration<T>(string name, out T value);

        string StartingLocation { get; }

        string ResolvePath(string name);
    }
}