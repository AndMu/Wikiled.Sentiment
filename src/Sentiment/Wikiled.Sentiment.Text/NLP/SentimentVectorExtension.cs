using Wikiled.Arff.Normalization;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.Sentiment.Text.Anomaly;
using Wikiled.Text.Analysis.NLP.NRC;

namespace Wikiled.Sentiment.Text.NLP
{
    public static class SentimentVectorExtension
    {
        public static VectorData GetVector(this SentimentVector vector, NormalizationType normalization)
        {
            return vector.GetTree(false).CreateVector(normalization, false);
        }
    }
}
