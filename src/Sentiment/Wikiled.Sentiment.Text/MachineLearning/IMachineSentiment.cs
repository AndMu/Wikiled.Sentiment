using Wikiled.MachineLearning.Mathematics.Vectors;

namespace Wikiled.Sentiment.Text.MachineLearning
{
    public interface IMachineSentiment
    {
        void Save(string path);

        (double Probability, double Normalization, VectorData Vector) GetVector(TextVectorCell[] cells);
    }
}
