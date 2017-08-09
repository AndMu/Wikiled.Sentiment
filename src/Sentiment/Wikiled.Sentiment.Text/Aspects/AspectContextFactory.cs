using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Aspects
{
    public class AspectContextFactory : IAspectContextFactory
    {
        public IAspectContext Create(IWordItem[] words)
        {
            Guard.NotNull(() => words, words);
            return new AspectContext(false, words);
        }
    }
}
