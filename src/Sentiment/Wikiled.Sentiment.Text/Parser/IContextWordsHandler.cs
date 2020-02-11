using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Text.Parser
{
    public interface IContextWordsHandler : IWordsHandler
    {
        ISessionContext Context { get; }
    }
}