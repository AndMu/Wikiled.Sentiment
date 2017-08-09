using Wikiled.Sentiment.Analysis.Anomaly;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class AnomalyDataFilter : IDataFilter
    {
        private readonly IProcessingDataAnomaly processing;

        public AnomalyDataFilter(IProcessingDataAnomaly processing)
        {
            this.processing = processing;
        }

        public bool CanInclude(SingleProcessingData data)
        {
            return !processing.IsAnomaly(data);
        }
    }
}
