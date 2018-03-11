using Wikiled.MachineLearning.Mathematics.Vectors;

namespace Wikiled.Sentiment.Text.MachineLearning
{
    public interface IMachineSentiment
    {
        void Save(string path);

        VectorData GetVector(TextVectorCell[] cells);
    }
}
