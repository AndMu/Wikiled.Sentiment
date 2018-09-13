using System;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.NLP
{
    public class SimpleSplitterFactory : ISplitterFactory
    {
        private readonly IWordsHandler wordsHandler;

        public SimpleSplitterFactory(ILexiconContainerFactory containerFactory)
        {
            if (containerFactory is null)
            {
                throw new ArgumentNullException(nameof(containerFactory));
            }

            wordsHandler = containerFactory.Construct();
        }

        public ITextSplitter ConstructSingle()
        {
            return new SimpleTextSplitter(wordsHandler);
        }

        public void Dispose()
        {
        }
    }
}
