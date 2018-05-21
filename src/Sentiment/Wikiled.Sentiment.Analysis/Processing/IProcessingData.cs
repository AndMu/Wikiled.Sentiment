using System.Collections.Generic;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public interface IProcessingData
    {
        IEnumerable<SingleProcessingData> Positive { get; }

        IEnumerable<SingleProcessingData> Negative { get; }

        IEnumerable<SingleProcessingData> Neutral { get; }
    }
}