using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Arff.Extensions;
using Wikiled.Arff.Logic;
using Wikiled.Arff.Logic.Headers;
using Wikiled.Common.Logging;
using Wikiled.MachineLearning.Mathematics;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.MachineLearning.Normalization;
using Wikiled.Sentiment.Text.Extensions;

namespace Wikiled.Sentiment.Text.MachineLearning
{
    public class MachineSentiment : IMachineSentiment
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<MachineSentiment>();

        private readonly Dictionary<IHeader, int> featureTable;

        private readonly double[] weights;

        public MachineSentiment(IArffDataSet dataSet, IClassifier classifier)
        {
            if (dataSet is null)
            {
                throw new ArgumentNullException(nameof(dataSet));
            }

            dataSet.Header.CreateHeader = false;
            DataSet = dataSet;
            Classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            weights = classifier.Model.ToWeights().Skip(1).ToArray();
            featureTable = dataSet.GetFeatureTable();
        }

        public IArffDataSet DataSet { get; }

        public IClassifier Classifier { get; }

        public static IMachineSentiment Load(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(path));
            }

            log.LogInformation("Loading {0}...", path);
            IArffDataSet reviews = ArffDataSet.Load<PositivityType>(Path.Combine(path, "data.arff"));
            var classifier = new Classifier();
            classifier.Load(Path.Combine(path, "training.model"));
            return new MachineSentiment(reviews, classifier);
        }

        public static async Task<MachineSentiment> Train(IArffDataSet arff, CancellationToken token)
        {
            log.LogInformation("Training SVM...");
            var classifier = new Classifier();
            (int? Y, double[] X)[] data = arff.GetDataNormalized(NormalizationType.L2).ToArray();
            if (data.Length < 40)
            {
                throw new ArgumentOutOfRangeException("Not enough training records");
            }

            var classCount = new Dictionary<int, int>();
            foreach ((int? Y, double[] X) datRecord in data)
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
                for (var i = 0; i < datRecord.X.Length; i++)
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
            Array[] randomized = GlobalSettings.Random.Shuffle(yData, xData).ToArray();
            await Task.Run(() => classifier.Train(randomized[0].Cast<int>().ToArray(), randomized[1].Cast<double[]>().ToArray(), token), token).ConfigureAwait(false);
            return new MachineSentiment(arff, classifier);
        }

        public (double Probability, double Normalization, VectorData Vector) GetVector(TextVectorCell[] cells)
        {
            if (cells is null)
            {
                throw new ArgumentNullException(nameof(cells));
            }

            log.LogDebug("GetVector");
            var vectorCells = new List<VectorCell>();
            var vector = new double[featureTable.Count];
            for (var i = 0; i < featureTable.Count; i++)
            {
                vector[i] = 0;
            }

            var unknownIndexes = vector.Length;
            foreach (TextVectorCell textCell in cells)
            {
                VectorCell cell = GetCell(textCell);
                if (cell != null)
                {
                    vector[cell.Index] = cell.X;
                    vectorCells.Add(cell);
                }
                else
                {
                    // if inverted exist in database, it is very likely that normal version has opposite meaning
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

            INormalize normalized = vector.Normalize(NormalizationType.L2);
            vector = normalized.GetNormalized.ToArray();
            var probability = Classifier.Probability(vector);

            // do not normalize data - SVM operates with normalized already. Second time normalization is not required.
            return (probability, normalized.Coeficient, new VectorData(vectorCells.ToArray(), unknownIndexes, Classifier.Model.Threshold, NormalizationType.None));
        }

        private VectorCell GetCell(TextVectorCell textCell)
        {
            IHeader header = DataSet.Header[textCell.Name];
            if (header == null ||
                !featureTable.TryGetValue(header, out var index))
            {
                return null;
            }

            TextVectorCell absoluteCell = textCell.Item == null
                ? new TextVectorCell(textCell.Name, Math.Abs(textCell.Value))
                : new TextVectorCell(textCell.Item, Math.Abs(textCell.Value));

            var cellItem = new VectorCell(index, absoluteCell, weights[index]);
            return cellItem;
        }

        public void Save(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(path));
            }

            log.LogInformation("Saving {0}...", path);
            DataSet.Save(Path.Combine(path, "data.arff"));
            Classifier.Save(Path.Combine(path, "training.model"));
            SaveCoef(Path.Combine(path, "coef.dat"));
        }

        private void SaveCoef(string fileName)
        {
            log.LogInformation("Saving {0}...", fileName);
            if (Classifier.Model == null)
            {
                return;
            }

            using (var stream = new StreamWriter(fileName, false))
            {
                foreach (KeyValuePair<IHeader, int> feature in featureTable)
                {
                    stream.WriteLine("{0} - {1}", feature.Key.Name, weights[feature.Value] + Classifier.Model.Threshold / DataSet.Header.Total);
                }
            }
        }
    }
}
