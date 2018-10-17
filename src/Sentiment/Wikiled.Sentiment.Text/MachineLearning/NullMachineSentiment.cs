using Wikiled.MachineLearning.Mathematics.Vectors;

namespace Wikiled.Sentiment.Text.MachineLearning
{
    public class NullMachineSentiment : IMachineSentiment
    {
        public void Save(string path)
        {
        }

        public (double Probability, double Normalization, VectorData Vector) GetVector(TextVectorCell[] cells)
        {
            return (0, 1, null);
        }

        public void SetAspectFilter(string path)
        {
        }
    }
}
