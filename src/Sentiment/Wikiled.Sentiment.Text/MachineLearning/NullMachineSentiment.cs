using System.Collections.Generic;
using Wikiled.Arff.Normalization;
using Wikiled.Arff.Persistence;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.MachineLearning.Svm.Logic;

namespace Wikiled.Sentiment.Text.MachineLearning
{
    public class NullMachineSentiment : IMachineSentiment
    {
        public NullMachineSentiment()
        {
            Header = new TrainingHeader();
        }

        public IArffDataSet Arff { get; }

        public TrainingHeader Header { get; }

        public void Save(string path)
        {
        }

        public double? Predict(VectorData vector)
        {
            return null;
        }

        public VectorData GetVector(IList<TextVectorCell> cells, NormalizationType normalizationType)
        {
            return null;
        }

        public MachineDetectionResult CalculateRating(VectorData vector)
        {
            return null;
        }
    }
}
