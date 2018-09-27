namespace Wikiled.Sentiment.Text.Configuration
{
    public interface ISentimentContextSource
    {
        ISessionContext Current { get; }
    }
}

