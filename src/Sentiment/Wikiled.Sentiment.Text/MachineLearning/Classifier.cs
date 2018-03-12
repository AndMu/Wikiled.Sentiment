using System;
using System.Threading;
using Accord.IO;
using Accord.MachineLearning;
using Accord.MachineLearning.Performance;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math.Optimization.Losses;
using Accord.Statistics.Kernels;
using NLog;

namespace Wikiled.Sentiment.Text.MachineLearning
{
    public class Classifier : IClassifier
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public SupportVectorMachine<Linear> Model { get; private set;}

        public void Train(int[] y, double[][] x, CancellationToken token)
        {
            log.Info("Training SVM...");
            var gridsearch = new GridSearch<SupportVectorMachine<Linear>, double[], int>
                                 {
                                     ParameterRanges =
                                         new GridSearchRangeCollection
                                             {
                                                 new GridSearchRange("complexity", new[] { 0.001, 0.01, 0.1, 1, 10 }),
                                             },
                                     Learner = p => new LinearDualCoordinateDescent { Complexity = p["complexity"], Loss = Loss.L2 },
                                     Loss = (actual, expected, m) => new ZeroOneLoss(expected).Loss(actual)
            };


            gridsearch.Token = token;
            var inputs = Accord.Statistics.Tools.Standardize(x);
            var result = gridsearch.Learn(inputs, y);
            Model = result.BestModel;
        }

        public void Save(string path)
        {
            log.Info("Save {0}", path);
            if (Model == null)
            {
                throw new InvalidOperationException("Model is not trained");
            }

            Model.Save(path);
        }

        public void Load(string path)
        {
            log.Info("Loading {0}", path);
            Model = Serializer.Load<SupportVectorMachine<Linear>>(path);
        }

        public double[] Probability(double[][] x)
        {
            log.Info("Probability...");
            if (Model == null)
            {
                throw new InvalidOperationException("Model is not trained");
            }

            var inputs = Accord.Statistics.Tools.Standardize(x);
            return Model.Probability(inputs);
        }

        public double Probability(double[] x)
        {
            log.Info("Probability...");
            if (Model == null)
            {
                throw new InvalidOperationException("Model is not trained");
            }
            
            var inputs = Accord.Statistics.Tools.Standardize(x);
            return Model.Probability(inputs);
        }

        public bool[] Classify(double[][] x)
        {
            log.Info("Classify...");
            if (Model == null)
            {
                throw new InvalidOperationException("Model is not trained");
            }

            var inputs = Accord.Statistics.Tools.Standardize(x);
            return Model.Decide(inputs);
        }
    }
}
