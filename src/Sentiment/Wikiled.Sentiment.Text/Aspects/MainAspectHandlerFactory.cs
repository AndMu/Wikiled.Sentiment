using System;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.Aspects
{
    public class MainAspectHandlerFactory : IMainAspectHandlerFactory
    {
        private readonly IWordsHandler handler;

        public MainAspectHandlerFactory(IWordsHandler handler)
        {
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
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
