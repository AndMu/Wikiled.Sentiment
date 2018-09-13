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

            wordsHandler = containerFactory.WordsHandler;
        }

        public bool CanConstruct => !IsConstructed;

        public bool IsConstructed { get; private set; }

        public ITextSplitter TextSplitter { get; private set; }

        public void Construct()
        {
            TextSplitter = ConstructSingle();
            IsConstructed = true;
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
