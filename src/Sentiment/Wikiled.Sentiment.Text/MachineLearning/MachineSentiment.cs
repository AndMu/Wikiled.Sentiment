using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using NLog;
using NLog.Fluent;
using Wikiled.Arff.Normalization;
using Wikiled.Arff.Persistence;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Serialization;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.MachineLearning.Svm.Clients;
using Wikiled.MachineLearning.Svm.Data;
using Wikiled.MachineLearning.Svm.Logic;

namespace Wikiled.Sentiment.Text.MachineLearning
{
    public class MachineSentiment<T> : IMachineSentiment
    {
        private readonly int factor;

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, double> readedCoef = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        private readonly Model trainingModel;

        private SparseVector coefVector;

        private SparseMatrix svmVectors;

        private Vector<float> thetaVector;

        public MachineSentiment(IArffDataSet arff, Model model, TrainingHeader header)
        {
            Guard.NotNull(() => arff, arff);
            Guard.NotNull(() => model, model);
            Guard.NotNull(() => header, header);
            Arff = arff;
            arff.Header.CreateHeader = false;
            trainingModel = model;
            factor = trainingModel.ClassLabels == null || trainingModel.ClassLabels[0] == 1 ? 1 : -1;
            Header = header;
            LearnCoeficients();
        }

        public IArffDataSet Arff { get; }

        public TrainingHeader Header { get; }

        public static MachineSentiment<T> Load(string path)
        {
            log.Info("Loading {0}...", path);
            var reviews = ArffDataSet.Load<T>(Path.Combine(path, "data.arff"));
            var trainingModel = Model.Read(Path.Combine(path, "training.model"));
            var header = XDocument.Load(Path.Combine(path, "header.xml")).XmlDeserialize<TrainingHeader>();
            log.Info("Normalization {0}...", header.Normalization);
            return new MachineSentiment<T>(reviews, trainingModel, header);
        }

        public static async Task<MachineSentiment<T>> Train(IArffDataSet arff, TrainingHeader header, CancellationToken token)
        {
            log.Info("Training SVM...");
            SvmTrainClient trainClient = new SvmTrainClient(arff);
            var results = await trainClient.Train(header, token).ConfigureAwait(false);
            if (results == null)
            {
                throw new SvmException("No data to generate training model");
            }

            return new MachineSentiment<T>(arff, results.Model, header);
        }

        public Task<MachineSentiment<T>> Retrain(CancellationToken token)
        {
            log.Info("Retraining...");
            return Train(Arff, Header, token);
        }

        public MachineDetectionResult CalculateRating(VectorData vector)
        {
            Guard.NotNull(() => vector, vector);
            if (vector.Cells.Length == 0)
            {
                return null;
            }

            SvmResult result = new SvmResult();
            result.SvmDistance = factor * -1 * trainingModel.Rho[0] + vector.Cells.Select(item => item.Calculated).Sum();
            if (Math.Round(Math.Abs(result.SvmDistance.Value), 2) > 0.05)
            {
                if (result.IsPositive != IsPositive(vector))
                {
                    throw new SvmException("Mistmatch in positivity");
                }
            }

            return new MachineDetectionResult(vector, result);
        }

        public void FilterCoef(string fileName)
        {
            readedCoef.Clear();
            foreach (var line in File.ReadAllLines(fileName))
            {
                var items = line.Split(new[] { " - " }, StringSplitOptions.None);
                readedCoef.Add(items[0].Trim(), double.Parse(items[1]));
            }
        }

        public VectorData GetVector(IList<TextVectorCell> cells, NormalizationType normalizationType)
        {
            Guard.NotNull(() => cells, cells);
            List<VectorCell> vectorCells = new List<VectorCell>();
            foreach (var textCell in cells)
            {
                var header = Arff.Header[textCell.Name];
                if (header == null)
                {
                    continue;
                }

                if (readedCoef.Count > 0 &&
                   !readedCoef.ContainsKey(textCell.Name))
                {
                    continue;
                }

                var index = Arff.Header.GetIndex(header);
                var cellItem = new VectorCell(index, textCell, thetaVector[index] * factor);
                vectorCells.Add(cellItem);
            }

            var rho = factor * -1 * trainingModel.Rho[0];
            return new VectorData(vectorCells.ToArray(), Arff.Header.Total, rho, normalizationType);
        }

        public double? Predict(VectorData vector)
        {
            Guard.NotNull(() => vector, vector);
            if (vector.Cells.Length == 0)
            {
                Log.Debug("Vector is zero length - quiting");
                return null;
            }

            DataLine line = new DataLine(1);
            foreach (var svmData in vector.Cells)
            {
                line.AddValue(svmData.Index, svmData.X);
            }

            var problem = Problem.Read(new[] { line });
            PredictionResult prediction = Prediction.Predict(problem, trainingModel, false);
            if (prediction.Classes.Length == 0)
            {
                return null;
            }

            return prediction.Classes[0].Actual;
        }

        public void Save(string path)
        {
            log.Info("Saving {0}...", path);
            Arff.Save(Path.Combine(path, "data.arff"));
            Header.XmlSerialize(Path.Combine(path, "header.xml"));
            trainingModel.Write(Path.Combine(path, "training.model"));
            SaveCoef(Path.Combine(path, "coef.txt"));
        }

        private bool? IsPositive(VectorData vector)
        {
            var result = Predict(vector);
            if (result == null)
            {
                return null;
            }

            return result == 1;
        }

        private void LearnCoeficients()
        {
            svmVectors = new SparseMatrix(trainingModel.SupportVectors.Length, Arff.Header.Total);
            for (int i = 0; i < trainingModel.SupportVectors.Length; i++)
            {
                foreach (var node in trainingModel.SupportVectors[i])
                {
                    svmVectors[i, node.Index] = (float)node.Value;
                }
            }

            coefVector = new SparseVector(trainingModel.SupportVectorCoefficients[0].Length);
            for (int i = 0; i < trainingModel.SupportVectorCoefficients[0].Length; i++)
            {
                coefVector[i] = (float)trainingModel.SupportVectorCoefficients[0][i];
            }

            thetaVector = svmVectors.Transpose().Multiply(coefVector);
        }

        private void SaveCoef(string fileName)
        {
            log.Info("Saving {0}...", fileName);
            Dictionary<string, double> coefTable = new Dictionary<string, double>();

            for (int i = 0; i < thetaVector.Count; i++)
            {
                var coef = thetaVector[i] * factor;
                var header = Arff.Header.GetByIndex(i);
                coefTable[header.Name] = coef;
            }

            using (StreamWriter stream = new StreamWriter(fileName, false))
            {
                foreach (var item in coefTable.OrderByDescending(item => item.Value))
                {
                    stream.WriteLine("{0} - {1}", item.Key, item.Value);
                }
            }
        }
    }
}
