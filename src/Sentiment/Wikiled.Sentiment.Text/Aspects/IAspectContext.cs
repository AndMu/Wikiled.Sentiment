using System.Collections.Generic;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Aspects
{
    public interface IAspectContext
    {
        void Process();

        IEnumerable<IWordItem> GetFeatures();

        IEnumerable<IWordItem> GetAttributes();
    }
}