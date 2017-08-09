using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Wikiled.Arff.Normalization;
using Wikiled.Arff.Persistence;
using Wikiled.Core.Utility.Arguments;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Anomaly;
using Wikiled.Sentiment.Text.Async;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Tokenizer;

namespace Wikiled.Sentiment.Analysis.Anomaly
{
    public class ProcessingDataAnomaly : IProcessingDataAnomaly
    {
        private const double Cutoff = 0.2;

        private readonly CosineSimilarityDistance distanceLogic = new CosineSimilarityDistance();

        private readonly Dictionary<SingleProcessingData, double> allTableDistance = new Dictionary<SingleProcessingData, double>();

        private readonly IWordsHandler handler;

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<SingleProcessingData, VectorData> table = new Dictionary<SingleProcessingData, VectorData>();

        private readonly Dictionary<SingleProcessingData, double> tableDistance = new Dictionary<SingleProcessingData, double>();

        private readonly SimpleWordsExtraction wordsExtraction;

        public ProcessingDataAnomaly(IWordsHandler handler, IProcessingData originalData)
        {
            Guard.NotNull(() => handler, handler);
            Guard.NotNull(() => originalData, originalData);
            wordsExtraction = new SimpleWordsExtraction(SentenceTokenizer.Create(handler, true, false));
            OriginalData = originalData;
            this.handler = handler;
        }

        public IProcessingData Extracted { get; private set; }

        public VectorData NegativeAnomalyVector { get; private set; }

        public double NegativeCutoff { get; private set; }

        public IProcessingData OriginalData { get; }

        public VectorData PositiveAnomalyVector { get; private set; }

        public double PositiveCutoff { get; private set; }

        public IProcessingData Extract()
        {
            log.Debug("Extract");
            allTableDistance.Clear();
            table.Clear();
            tableDistance.Clear();
            Parallel.ForEach(
                OriginalData.AllReviews.Where(item => item.Document == null),
                AsyncSettings.DefaultParallel,
                review => review.InitDocument(wordsExtraction));
            ProcessingData data = new ProcessingData();

            log.Debug("Detecting Positive vector...");
            PositiveAnomalyVector = CreateVector(OriginalData.Positive);
            var positive = tableDistance.OrderByDescending(item => item.Value)
                                        .Take((int)((1 - Cutoff) * tableDistance.Count))
                                        .ToArray();
            PositiveCutoff = positive.Last().Value;
            data.Positive = positive.Select(item => item.Key).ToArray();

            tableDistance.Clear();
            log.Debug("Detecting Negative vector...");
            NegativeAnomalyVector = CreateVector(OriginalData.Negative);

            var negative = tableDistance.OrderByDescending(item => item.Value)
                                        .Take((int)((1 - Cutoff) * tableDistance.Count))
                                        .ToArray();
            NegativeCutoff = negative.Last().Value;
            data.Negative = negative.Select(item => item.Key).ToArray();
            return data;
        }

        public bool IsAnomaly(SingleProcessingData data)
        {
            var value = allTableDistance[data];
            double cutoff = data.Document.GetPositivity() == PositivityType.Positive ? PositiveCutoff : NegativeCutoff;
            return value < cutoff;
        }

        private VectorData CreateVector(SingleProcessingData[] reviews)
        {
            List<VectorData> vectors = new List<VectorData>();
            Parallel.ForEach(
                reviews,
                AsyncSettings.DefaultParallel,
                review =>
                {
                    ParsedReviewFactory factory = new ParsedReviewFactory(handler, review.Document);
                    var parsed = factory.Create();
                    DocumentAnomalyDetector dectector = new DocumentAnomalyDetector(
                        handler,
                        parsed);
                    VectorData fullVector = dectector.GetDocumentVector(NormalizationType.None);
                    lock (vectors)
                    {
                        vectors.Add(fullVector);
                        table[review] = fullVector;
                    }
                });

            var vector = vectors.Sum(NormalizationType.L2);
            log.Debug("Calculating distances...");
            Parallel.ForEach(
                reviews,
                AsyncSettings.DefaultParallel,
                review =>
                {
                    double value = distanceLogic.Measure(table[review], vector);
                    lock (tableDistance)
                    {
                        tableDistance[review] = value;
                        allTableDistance[review] = value;
                    }
                });

            return vector;
        }
    }
}
