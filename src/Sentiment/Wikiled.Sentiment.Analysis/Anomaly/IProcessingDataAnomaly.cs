using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Anomaly
{
    public interface IProcessingDataAnomaly
    {
        IProcessingData Extract();

        bool IsAnomaly(SingleProcessingData data);

        VectorData PositiveAnomalyVector { get; }

        VectorData NegativeAnomalyVector { get; }

        IProcessingData OriginalData { get; }

        IProcessingData Extracted { get; }

        double PositiveCutoff { get; }

        double NegativeCutoff { get; }
    }
}