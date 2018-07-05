using Wikiled.Common.Arguments;
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
            this.wordsHandlersManager = wordsHandlersManager ?? throw new System.ArgumentNullException(nameof(wordsHandlersManager));
        }

        public IWordItem Construct(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                throw new System.ArgumentException("message", nameof(word));
            }

            BasePOSType wordPOSType = wordsHandlersManager.PosTagger.GetTag(word);
            return wordsHandlersManager.WordFactory.CreateWord(
                word,
                wordPOSType);
        }
    }
}
