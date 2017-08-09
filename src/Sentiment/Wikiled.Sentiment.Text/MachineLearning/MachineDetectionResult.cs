using Wikiled.Core.Utility.Arguments;
using Wikiled.MachineLearning.Mathematics.Vectors;

namespace Wikiled.Sentiment.Text.MachineLearning
{
    public class MachineDetectionResult
    {
        public MachineDetectionResult(VectorData vector, SvmResult result)
        {
            Guard.NotNull(() => vector, vector);
            Guard.NotNull(() => result, result);
            Vector = vector;
            Result = result;
        }

        public SvmResult Result { get; }

        public VectorData Vector { get; }
    }
}
