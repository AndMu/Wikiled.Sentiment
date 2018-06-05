using System.Collections.Generic;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Data
{
    public interface ISentence
    {
        ISentencePart CurrentPart { get; }

        int Index { get; }

        ISentence Next { get; set; }

        IParsedReview Review { get; }

        IEnumerable<IWordItem> Occurrences { get; }

        IEnumerable<ISentencePart> Parts { get; }

        ISentence Previous { get; set; }

        string Text { get; }

        void CreateNewPart();
    }
}
