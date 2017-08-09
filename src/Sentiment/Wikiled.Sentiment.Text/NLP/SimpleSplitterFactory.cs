using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.NLP
{
    public class SimpleSplitterFactory : ISplitterFactory
    {
        private readonly IWordsHandler wordsHandler;

        public SimpleSplitterFactory(ILexiconFactory factory)
        {
            Guard.NotNull(() => factory, factory);
            wordsHandler = factory.WordsHandler;
        }

        public void Construct()
        {
            TextSplitter = ConstructSingle();
            IsConstructed = true;
        }

        public bool CanConstruct => !IsConstructed;

        public bool IsConstructed { get; private set; }

        public ITextSplitter TextSplitter { get; private set; }

        public ITextSplitter ConstructSingle()
        {
            return new SimpleTextSplitter(wordsHandler);
        }

        public void Dispose()
        {
        }
    }
}
