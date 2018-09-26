using Wikiled.Sentiment.Text.Configuration;

namespace Wikiled.Sentiment.Text.Parser
{
    public interface IContextWordsHandler : IWordsHandler
    {
        ISentimentContext Context { get; }
    }
}