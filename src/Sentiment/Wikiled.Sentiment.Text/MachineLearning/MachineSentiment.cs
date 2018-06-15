using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Wikiled.Arff.Extensions;
using Wikiled.Arff.Persistence;
using Wikiled.Arff.Persistence.Headers;
using Wikiled.Common.Arguments;
using Wikiled.MachineLearning.Mathematics;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.MachineLearning.Normalization;
using Wikiled.Sentiment.Text.Extensions;

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
            weights = classifier.Model.ToWeights().Skip(1).ToArray();
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
            var data = arff.GetDataNormalized(NormalizationType.L2).ToArray();
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
                    datRecord.X[i] = Math.Abs(x);
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

            var yData = data.Select(item => item.Y.Value).ToArray();
            var xData = data.Select(item => item.X).ToArray();
            var randomized = new Random().Shuffle(yData, xData).ToArray();
            await Task.Run(() => classifier.Train(randomized[0].Cast<int>().ToArray(), randomized[1].Cast<double[]>().ToArray(), token), token).ConfigureAwait(false);
            return new MachineSentiment(arff, classifier);
        }

        public (double Probability, VectorData Vector) GetVector(TextVectorCell[] cells)
        {
            Guard.NotNull(() => cells, cells);
            log.Debug("GetVector");
            List<VectorCell> vectorCells = new List<VectorCell>();
            double[] vector = new double[featureTable.Count];
            for (int i = 0; i < featureTable.Count; i++)
            {
                vector[i] = 0;
            }

            var unknownIndexes = vector.Length;
            foreach (var textCell in cells)
            {
                var cell = GetCell(textCell);
                if (cell != null)
                {
                    vector[cell.Index] = cell.X;
                    vectorCells.Add(cell);
                }
                else
                {
                    // if invereted exist in database, it is very likely that normal version has opposite meaning
                    cell = GetCell(new TextVectorCell(textCell.Name.GetOpposite(), Math.Abs(textCell.Value)));
                    if (cell != null)
                    {
                        var theata = textCell.Name.IsInverted() ? cell.Theta / 2 : cell.Theta / 4;
                        cell = new VectorCell(unknownIndexes, textCell, -theata);
                        vectorCells.Add(cell);
                        unknownIndexes++;
                    }
                }
            }

            vector = vector.Normalize(NormalizationType.L2).GetNormalized.ToArray();
            var probability = Classifier.Probability(vector);
            return (probability, new VectorData(vectorCells.ToArray(), unknownIndexes, Classifier.Model.Threshold, NormalizationType.L2));
        }

        private VectorCell GetCell(TextVectorCell textCell)
        {
            var header = DataSet.Header[textCell.Name];
            if (header == null ||
                !featureTable.TryGetValue(header, out var index))
            {
                return null;
            }

            var absoluteCell = textCell.Item == null
                ? new TextVectorCell(textCell.Name, Math.Abs(textCell.Value))
                : new TextVectorCell(textCell.Item, Math.Abs(textCell.Value));

            var cellItem = new VectorCell(index, absoluteCell, weights[index]);
            return cellItem;
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
