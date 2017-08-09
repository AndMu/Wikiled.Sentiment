using System.Collections.Generic;

namespace Wikiled.Sentiment.Text.Words
{
    public interface IPhrase : IWordItem
    {
        IEnumerable<IWordItem> AllWords { get; }

        void Add(IWordItem word);

        int NonStopWordCount { get; }
    }
}