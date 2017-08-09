using Wikiled.Core.Utility.Arguments;
using Wikiled.Text.Analysis.POS.Tags;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Tokenizer
{
    public class SimpleWordItemFactory : IWordItemFactory
    {
        private readonly IWordsHandler wordsHandlersManager;

        public SimpleWordItemFactory(IWordsHandler wordsHandlersManager)
        {
            Guard.NotNull(() => wordsHandlersManager, wordsHandlersManager);
            this.wordsHandlersManager = wordsHandlersManager;
        }

        public IWordItem Construct(string word)
        {
            Guard.NotNullOrEmpty(() => word, word);
            BasePOSType wordPOSType = wordsHandlersManager.PosTagger.GetTag(word);
            return wordsHandlersManager.WordFactory.CreateWord(
                word,
                wordPOSType);
        }
    }
}
