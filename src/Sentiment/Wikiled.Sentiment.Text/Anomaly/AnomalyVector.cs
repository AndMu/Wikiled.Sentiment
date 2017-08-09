using System;
using Wikiled.Arff.Normalization;
using Wikiled.MachineLearning.Mathematics.Vectors;

namespace Wikiled.Sentiment.Text.Anomaly
{
    public class AnomalyVector
    {
        public NormalizationType Normalization { get; set; }

        public VectorData GetData(AnomalyVectorType type)
        {
            switch (type)
            {
                case AnomalyVectorType.Full:
                    return Full;
                case AnomalyVectorType.Inquirer:
                    return Short;
                case AnomalyVectorType.SentimentCategory:
                    return SentimentCategory;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        public VectorData SentimentCategory { get; set; }

        public VectorData Full { get; set; }

        public VectorData Short { get; set; }
    }
}
