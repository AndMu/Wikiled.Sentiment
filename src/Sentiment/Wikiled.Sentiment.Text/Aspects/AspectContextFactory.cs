using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Aspects
{
    public class AspectContextFactory : IAspectContextFactory
    {
        private readonly bool includeSentiment;

        public AspectContextFactory(bool includeSentiment = false)
        {
            this.includeSentiment = includeSentiment;
        }

        public IAspectContext Create(IWordItem[] words)
        {
            if (words == null)
            {
                throw new System.ArgumentNullException(nameof(words));
            }

            return new AspectContext(includeSentiment, words);
        }
    }
}
