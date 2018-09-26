namespace Wikiled.Sentiment.Text.Configuration
{
    public interface ISentimentContextSource
    {
        ISentimentContext Current { get; }
    }
}

