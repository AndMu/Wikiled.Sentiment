using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Tokenizer
{
    public interface IWordItemFactory
    {
        IWordItem Construct(string word);
    }
}