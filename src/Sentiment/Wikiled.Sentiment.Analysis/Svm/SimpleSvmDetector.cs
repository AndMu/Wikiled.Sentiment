using Wikiled.Arff.Normalization;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Svm
{
    public class SimpleSvmDetector : ISvmDetector
    {
        private readonly IMachineSentiment classifier;

        private readonly NormalizationType normalizationType;

        public SimpleSvmDetector(IMachineSentiment classifier, NormalizationType normalizationType)
        {
            Guard.NotNull(() => classifier, classifier);
            this.normalizationType = normalizationType;
            this.classifier = classifier;
        }

        public MachineDetectionResult Evaluate(Document document)
        {
            Guard.NotNull(() => document, document);
            var vector = classifier.GetVector(document.GetCellsOccurenceOnly(), normalizationType);
            var results = classifier.CalculateRating(vector);
            document.PopulateResults(results);
            return results;
        }
    }
}
