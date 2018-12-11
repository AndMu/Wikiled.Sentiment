using Accord.IO;
using Accord.MachineLearning;
using Accord.MachineLearning.Performance;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math.Optimization.Losses;
using Accord.Statistics.Kernels;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using Wikiled.Common.Logging;

namespace Wikiled.Sentiment.Text.MachineLearning
{
    public class Classifier : IClassifier
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<Classifier>();

        public SupportVectorMachine Model { get; private set; }

        public void Train(int[] y, double[][] x, CancellationToken token)
        {
            log.LogInformation("Training SVM...");
            GridSearch<SupportVectorMachine, double[], int> gridsearch = new GridSearch<SupportVectorMachine, double[], int>
            {
                ParameterRanges =
                                         new GridSearchRangeCollection
                                             {
                                                 //new GridSearchRange("complexity", new[] { 0.001, 0.01, 0.1, 1, 10, 100, 1000 }),
                                                 new GridSearchRange("C", new[] { 0.001, 0.01, 0.1, 1, 10 }),
                                             },
                Learner = p => new LinearDualCoordinateDescent
                {
                    //Complexity = p["complexity"],
                    Loss = Loss.L2,
                    Kernel = new Linear(p["C"])
                },
                Loss = (actual, expected, m) => new ZeroOneLoss(expected).Loss(actual)
            };

            gridsearch.Token = token;
            GridSearchResult<SupportVectorMachine, double[], int> result = gridsearch.Learn(x, y);
            Model = result.BestModel;
            GridSearchParameterCollection parameters = result.BestParameters;
            double error = result.BestModelError;
            log.LogInformation("SVM Trained. Threshold: {0} Constant: {1} Error: {2} ...", Model.Threshold, parameters[0].Value, error);
        }

        public void Save(string path)
        {
            log.LogInformation("Save {0}", path);
            if (Model == null)
            {
                throw new InvalidOperationException("Model is not trained");
            }

            Model.Save(path);
        }

        public void Load(string path)
        {
            log.LogInformation("Loading {0}", path);
            Model = Serializer.Load<SupportVectorMachine>(path);
        }

        public double[] Probability(double[][] x)
        {
            log.LogInformation("Probability...");
            if (Model == null)
            {
                throw new InvalidOperationException("Model is not trained");
            }

            return Model.Probability(x);
        }

        public double Probability(double[] x)
        {
            log.LogInformation("Probability...");
            if (Model == null)
            {
                throw new InvalidOperationException("Model is not trained");
            }

            return Model.Probability(x);
        }

        public bool[] Classify(double[][] x)
        {
            log.LogInformation("Classify...");
            if (Model == null)
            {
                throw new InvalidOperationException("Model is not trained");
            }

            return Model.Decide(x);
        }
    }
}
