using System;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.NLP
{
    public class SimpleSplitterFactory : ISplitterFactory
    {
        private readonly IWordsHandler wordsHandler;

        public SimpleSplitterFactory(ILexiconFactory factory)
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            wordsHandler = factory.WordsHandler;
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
