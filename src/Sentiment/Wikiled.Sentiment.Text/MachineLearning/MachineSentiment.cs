using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Wikiled.Arff.Normalization;
using Wikiled.Arff.Persistence;
using Wikiled.Arff.Persistence.Headers;
using Wikiled.Common.Arguments;
using Wikiled.MachineLearning.Mathematics.Vectors;

namespace Wikiled.Sentiment.Text.MachineLearning
{
    public class MachineSentiment : IMachineSentiment
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<IHeader, int> featureTable;

        private readonly double[] weights;

        public MachineSentiment(IArffDataSet dataSet, IClassifier classifier)
        {
            Guard.NotNull(() => dataSet, dataSet);
            Guard.NotNull(() => classifier, classifier);
            dataSet.Header.CreateHeader = false;
            DataSet = dataSet;
            Classifier = classifier;
            weights = classifier.Model.ToWeights();
            featureTable = dataSet.GetFeatureTable();
        }

        public IArffDataSet DataSet { get; }

        public IClassifier Classifier { get; }

        public static IMachineSentiment Load(string path)
        {
            log.Info("Loading {0}...", path);
            Guard.NotNull(() => path, path);
            var reviews = ArffDataSet.Load<PositivityType>(Path.Combine(path, "data.arff"));
            Classifier classifier = new Classifier();
            classifier.Load(Path.Combine(path, "training.model"));
            return new MachineSentiment(reviews, classifier);
        }

        public static async Task<MachineSentiment> Train(IArffDataSet arff, CancellationToken token)
        {
            log.Info("Training SVM...");
            Classifier classifier = new Classifier();
            arff.Normalize(NormalizationType.L2);
            var data = arff.GetData().ToArray();
            if (data.Length < 40)
            {
                throw new ArgumentOutOfRangeException("Not enough training records");
            }

            Dictionary<int, int> classCount = new Dictionary<int, int>();
            foreach (var datRecord in data)
            {
                if (classCount.ContainsKey(datRecord.Y.Value))
                {
                    classCount[datRecord.Y.Value] += 1;
                }
                else
                {
                    classCount[datRecord.Y.Value] = 1;
                }

                // Make all sentiments positive - counts with weights
                for (int i = 0; i < datRecord.X.Length; i++)
                {
                    var x = datRecord.X[i];
                    datRecord.X[i] = x >= 0 ? x : -x;
                }
            }

            if (classCount.Count != 2)
            {
                throw new ArgumentOutOfRangeException("Two classes not found");
            }

            if (classCount[-1] < 20)
            {
                throw new ArgumentOutOfRangeException("Not enough negative classes");
            }

            if (classCount[1] < 20)
            {
                throw new ArgumentOutOfRangeException("Not enough positive classes");
            }

            await Task.Run(() => classifier.Train(data.Select(item => item.Y.Value).ToArray(), data.Select(item => item.X).ToArray(), token), token).ConfigureAwait(false);
            return new MachineSentiment(arff, classifier);
        }

        public (double Probability, VectorData Vector) GetVector(TextVectorCell[] cells)
        {
            Guard.NotNull(() => cells, cells);
            log.Debug("GetVector");
            List<VectorCell> vectorCells = new List<VectorCell>();
            double[] vector = new double[DataSet.Header.Total];
            for (int i = 0; i < DataSet.Header.Total; i++)
            {
                vector[i] = 0;
            }

            foreach (var textCell in cells)
            {
                var header = DataSet.Header[textCell.Name];
                if (header == null ||
                    !featureTable.TryGetValue(header, out var index))
                {
                    continue;
                }

                vector[index] = textCell.Value;
                var cellItem = new VectorCell(index, textCell, weights[index]);
                vectorCells.Add(cellItem);
            }

            var probability = Classifier.Probability(vector);
            return (probability, new VectorData(vectorCells.ToArray(), DataSet.Header.Total, Classifier.Model.Threshold, NormalizationType.None));
        }

        public void Save(string path)
        {
            Guard.NotNull(() => path, path);
            log.Info("Saving {0}...", path);
            DataSet.Save(Path.Combine(path, "data.arff"));
            Classifier.Save(Path.Combine(path, "training.model"));
            SaveCoef(Path.Combine(path, "coef.dat"));
        }

        private void SaveCoef(string fileName)
        {
            log.Info("Saving {0}...", fileName);
            if (Classifier.Model == null)
            {
                return;
            }

            using (StreamWriter stream = new StreamWriter(fileName, false))
            {
                foreach (var feature in featureTable)
                {
                    stream.WriteLine("{0} - {1}", feature.Key.Name, weights[feature.Value] + Classifier.Model.Threshold / DataSet.Header.Total);
                }
            }
        }
    }
}
