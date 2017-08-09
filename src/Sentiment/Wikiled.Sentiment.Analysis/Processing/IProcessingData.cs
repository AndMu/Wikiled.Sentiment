using System.Collections.Generic;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public interface IProcessingData : IReviewSource
    {
        SingleProcessingData[] Positive { get; }

        SingleProcessingData[] Negative { get; }

        SingleProcessingData[] Neutral { get; }
    }
}