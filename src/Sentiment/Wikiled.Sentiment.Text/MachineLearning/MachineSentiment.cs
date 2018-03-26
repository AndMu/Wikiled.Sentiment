using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Wikiled.Arff.Normalization;
using Wikiled.Arff.Persistence;
using Wikiled.Common.Arguments;
using Wikiled.MachineLearning.Mathematics.Vectors;

namespace Wikiled.Sentiment.Text.MachineLearning
{
    public class MachineSentiment : IMachineSentiment
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IArffDataSet dataSet;

        private readonly IClassifier classifier;

        private readonly double[] weights;

        public MachineSentiment(IArffDataSet dataSet, IClassifier classifier)
        {
            Guard.NotNull(() => dataSet, dataSet);
            Guard.NotNull(() => classifier, classifier);
            dataSet.Header.CreateHeader = false;
            this.dataSet = dataSet;
            this.classifier = classifier;
            weights = classifier.Model.ToWeights();
        }

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
            var data = arff.GetData().ToArray();
            await Task.Run(() => classifier.Train(data.Select(item => item.Y).ToArray(), data.Select(item => item.X).ToArray(), token), token).ConfigureAwait(false);
            return new MachineSentiment(arff, classifier);
        }

        public void Save(string path)
        {
            Guard.NotNull(() => path, path);
            log.Info("Saving {0}...", path);
            dataSet.Save(Path.Combine(path, "data.arff"));
            classifier.Save(Path.Combine(path, "training.model"));
            SaveCoef(Path.Combine(path, "coef.dat"));
        }

        public (double Probability, VectorData Vector) GetVector(TextVectorCell[] cells)
        {
            Guard.NotNull(() => cells, cells);
            log.Debug("GetVector");
            List<VectorCell> vectorCells = new List<VectorCell>();
            double[] vector = new double[dataSet.Header.Total];
            for (int i = 0; i < dataSet.Header.Total; i++)
            {
                vector[i] = 0;
            }

            foreach (var textCell in cells)
            {
                var header = dataSet.Header[textCell.Name];
                if (header == null)
                {
                    continue;
                }

                var index = dataSet.Header.GetIndex(header);
                vector[index] = textCell.Value;
                var cellItem = new VectorCell(index, textCell, weights[index]);
                vectorCells.Add(cellItem);
            }

            var probability = classifier.Probability(vector);
            return (probability, new VectorData(vectorCells.ToArray(), dataSet.Header.Total, classifier.Model.Threshold, NormalizationType.None));
        }

        private void SaveCoef(string fileName)
        {
            log.Info("Saving {0}...", fileName);
            if (classifier.Model == null)
            {
                return;
            }
            
            using (StreamWriter stream = new StreamWriter(fileName, false))
            {
                for (int i = 0; i < dataSet.Header.Total; i++)
                {
                    stream.WriteLine("{0} - {1}", dataSet.Header.GetByIndex(i).Name, weights[i] + classifier.Model.Threshold / dataSet.Header.Total);
                }
            }
        }
    }
}
