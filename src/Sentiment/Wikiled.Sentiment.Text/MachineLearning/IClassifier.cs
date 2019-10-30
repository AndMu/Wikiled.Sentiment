using Accord.MachineLearning.VectorMachines;
using System.Threading;
using System.Threading.Tasks;

namespace Wikiled.Sentiment.Text.MachineLearning
{
    public interface IClassifier
    {
        ParallelOptions Options { get; set; }

        SupportVectorMachine Model { get; }

        void Train(int[] y, double[][] x, CancellationToken token);

        void Save(string path);

        void Load(string path);

        bool[] Classify(double[][] x);

        double[] Probability(double[][] x);

        double Probability(double[] x);
    }
}