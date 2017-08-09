using System.Collections.Generic;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Aspects
{
    public interface IAspectDectector
    {
        void Remove(IWordItem feature);

        void AddFeature(IWordItem feature);

        bool IsAspect(IWordItem word);

        bool IsAttribute(IWordItem word);

        IEnumerable<IWordItem> AllFeatures { get; }

        IEnumerable<IWordItem> AllAttributes { get; }
    }
}