namespace Wikiled.Sentiment.Text.Configuration
{
    public interface IConfigurationFactory
    {
        void Construct();

        bool CanConstruct { get; }

        bool IsConstructed { get; }
    }
}
