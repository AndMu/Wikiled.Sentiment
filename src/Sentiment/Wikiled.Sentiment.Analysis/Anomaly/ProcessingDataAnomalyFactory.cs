using Wikiled.Sentiment.Analysis.Processing;

namespace Wikiled.Sentiment.Analysis.Anomaly
{
    public class ProcessingDataAnomalyFactory
    {
        public static readonly ProcessingDataAnomalyFactory Instance = new ProcessingDataAnomalyFactory();

        private ProcessingDataAnomalyFactory()
        {
        }

        public IProcessingDataAnomaly Create(IProcessingData data)
        {
            return new NullProcessingDataAnomaly(data);
        }
    }
}
