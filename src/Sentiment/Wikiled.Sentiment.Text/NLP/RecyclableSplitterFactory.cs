using System;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.NLP
{
    public class RecyclableSplitterFactory : ISplitterFactory
    {
        private readonly ISplitterFactory factory;

        public RecyclableSplitterFactory(ISplitterFactory factory)
        {
            Guard.NotNull(() => factory, factory);
            this.factory = factory;
        }

        public void Construct()
        {
            factory.Construct();
        }

        public bool CanConstruct => factory.CanConstruct;

        public bool IsConstructed => factory.IsConstructed;

        public void Dispose()
        {
            factory.Dispose();
        }

        public ITextSplitter ConstructSingle()
        {
            return new RecyclableTextSplitter(factory);
        }
    }
}
