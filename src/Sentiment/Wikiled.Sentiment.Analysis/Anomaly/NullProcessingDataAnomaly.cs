using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Anomaly
{
    public class NullProcessingDataAnomaly : IProcessingDataAnomaly
    {
        public NullProcessingDataAnomaly(IProcessingData originalData)
        {
            OriginalData = originalData;
        }

        public IProcessingData Extract()
        {
            Extracted = OriginalData;
            return OriginalData;
        }

        public bool IsAnomaly(SingleProcessingData data)
        {
            return false;
        }

        public VectorData PositiveAnomalyVector { get; private set; }

        public VectorData NegativeAnomalyVector { get; private set; }

        public IProcessingData OriginalData { get; }

        public IProcessingData Extracted { get; private set; }

        public double PositiveCutoff { get; private set; }

        public double NegativeCutoff { get; private set; }
    }
}
