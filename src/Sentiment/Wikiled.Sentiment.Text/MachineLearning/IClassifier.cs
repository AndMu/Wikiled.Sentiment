using System.Threading;
using Accord.MachineLearning.VectorMachines;
using Accord.Statistics.Kernels;

namespace Wikiled.Sentiment.Text.MachineLearning
{
    public interface IClassifier
    {
        SupportVectorMachine<Linear> Model { get; }

        void Train(int[] y, double[][] x, CancellationToken token);

        void Save(string path);

        void Load(string path);

        bool[] Classify(double[][] x);

        double[] Probability(double[][] x);

        double Probability(double[] x);
    }
}