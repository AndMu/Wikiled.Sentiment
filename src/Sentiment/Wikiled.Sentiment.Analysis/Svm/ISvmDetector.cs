using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Svm
{
    internal interface ISvmDetector
    {
        MachineDetectionResult Evaluate(Document text);
    }
}