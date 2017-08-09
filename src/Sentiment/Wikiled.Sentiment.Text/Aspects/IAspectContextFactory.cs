using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Aspects
{
    public interface IAspectContextFactory
    {
        IAspectContext Create(IWordItem[] words);
    }
}