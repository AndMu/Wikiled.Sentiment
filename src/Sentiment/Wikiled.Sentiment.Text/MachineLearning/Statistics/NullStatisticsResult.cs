using System.Collections.Generic;
using MathNet.Numerics.Statistics;

namespace Wikiled.Sentiment.Text.MachineLearning.Statistics
{
    class NullStatisticsResult : IStatisticsResult
    {
        public NullStatisticsResult(double value)
        {
            Value = value;
        }

        public void AddBucketLabels(int minimum, int step)
        {
        }

        public void AddData(double data)
        {
        }

        public void AddBucketData(int step, int total)
        {
        }

        public void AddBucketData(string label, double data)
        {
        }

        public void IncrementTotal()
        {
        }

        public void IncrementOccurences()
        {
        }

        public int WindowsSize
        {
            get { return 0; }
        }

        public double Value { get; private set; }

        public int TotalWords
        {
            get { return 0; }
        }

        public int TotalOccurences
        {
            get { return 0; }
        }

        public DescriptiveStatistics Statistics
        {
            get { return null; }
        }

        public IEnumerable<string> BucketLabels { get { return null; } }

        public IEnumerable<double> DataBag
        {
            get { return null; }
        }

        public Histogram Histogram { get { return null; } }

        public int Count
        {
            get { return 0; }
        }

        public int BucketCount
        {
            get { return 0; }
            set { }
        }
    }
}
