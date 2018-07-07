using Wikiled.Sentiment.Text.Data;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public class ContextSentimentFactory : IContextSentimentFactory
    {
        public IContextSentiment Construct(IWordItemRelationships parent)
        {
            if (parent == null)
            {
                throw new System.ArgumentNullException(nameof(parent));
            }

            var context = new ContextSentimentCalculator(parent);
            context.Process();
            return context;
        }
    }
}
