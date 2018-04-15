using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Aspects
{
    public class MainAspectHandlerFactory : IMainAspectHandlerFactory
    {
        private readonly IWordsHandler handler;

        public MainAspectHandlerFactory(IWordsHandler handler)
        {
            Guard.NotNull(() => handler, handler);
            this.handler = handler;
        }

        public IMainAspectHandler Construct()
        {
            return new MainAspectHandler(new AspectContextFactory());
        }

        public IAspectSerializer ConstructSerializer()
        {
            return new AspectSerializer(handler);
        }
    }
}
