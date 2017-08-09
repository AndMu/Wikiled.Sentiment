using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public interface IDataFilter
    {
        bool CanInclude(SingleProcessingData data);
    }
}