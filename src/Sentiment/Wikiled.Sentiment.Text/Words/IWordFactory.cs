using Wikiled.Text.Analysis.POS.Tags;

namespace Wikiled.Sentiment.Text.Words
{
    public interface IWordFactory
    {
        IPhrase CreatePhrase(string wordType);

        IWordItem CreateWord(string word, string wordType);

        IWordItem CreateWord(string word, string lemma, string wordType);

        IWordItem CreateWord(string word, BasePOSType wordPosType);
    }
}