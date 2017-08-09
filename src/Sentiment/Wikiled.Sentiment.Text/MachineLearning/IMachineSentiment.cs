using System.Collections.Generic;
using Wikiled.Arff.Normalization;
using Wikiled.Arff.Persistence;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.MachineLearning.Svm.Logic;

namespace Wikiled.Sentiment.Text.MachineLearning
{
    public interface IMachineSentiment
    {
        IArffDataSet Arff { get; }

        TrainingHeader Header { get; }

        void Save(string path);

        double? Predict(VectorData vector);

        VectorData GetVector(IList<TextVectorCell> cells, NormalizationType normalizationType);

        MachineDetectionResult CalculateRating(VectorData vector);
    }
}
