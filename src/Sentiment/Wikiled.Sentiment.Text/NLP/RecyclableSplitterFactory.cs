using System;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.NLP
{
    public class RecyclableSplitterFactory : ISplitterFactory
    {
        private readonly ISplitterFactory factory;

        public RecyclableSplitterFactory(ISplitterFactory factory)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

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
