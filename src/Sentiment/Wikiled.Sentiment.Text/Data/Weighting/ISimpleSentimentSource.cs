using System.Collections.Generic;

namespace Wikiled.Sentiment.Text.Data.Weighting
{
    public interface ISimpleSentimentSource
    {
        double? Measure(IWordItem item);
        IEnumerable<IWordItem> Positive { get; }
        IEnumerable<IWordItem> Negative { get; }
    }
}