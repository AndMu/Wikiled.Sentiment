using System.Collections.Generic;

namespace Wikiled.Sentiment.Text.Words
{
    public interface IPhraseContructor
    {
        IEnumerable<IPhrase> GetPhrases(IWordItem word);
    }
}