using System.Collections.Generic;
using Wikiled.MachineLearning.Mathematics.Vectors;

namespace Wikiled.Sentiment.Text.MachineLearning
{
    public class NullMachineSentiment : IMachineSentiment
    {
        public void Save(string path)
        {
        }

        public VectorData GetVector(TextVectorCell[] cells)
        {
            return null;
        }

        public void SetAspectFilter(string path)
        {
        }
    }
}
