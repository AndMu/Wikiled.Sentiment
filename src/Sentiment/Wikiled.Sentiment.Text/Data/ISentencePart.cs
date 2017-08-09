using System.Collections.Generic;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Data
{
    public interface ISentencePart
    {
        void AddItem(IWordItem item);

        IList<IWordItem> Occurrences { get; }

        ISentence Sentence { get; }

        ISentencePart Previous { get; }
    }
}