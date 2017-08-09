using System.Collections.Generic;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public interface IReviewSource
    {
        IEnumerable<SingleProcessingData> AllReviews { get; }
    }
}