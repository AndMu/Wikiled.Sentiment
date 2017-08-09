using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Data;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public class ContextSentimentFactory : IContextSentimentFactory
    {
        public IContextSentiment Construct(IWordItemRelationships parent)
        {
            Guard.NotNull(() => parent, parent);
            var context = new ContextSentimentCalculator(parent);
            context.Process();
            return context;
        }
    }
}
