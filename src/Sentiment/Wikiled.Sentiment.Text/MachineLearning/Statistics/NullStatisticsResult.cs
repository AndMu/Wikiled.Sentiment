using System.Collections.Generic;
using MathNet.Numerics.Statistics;

namespace Wikiled.Sentiment.Text.MachineLearning.Statistics
{
    public class NullStatisticsResult : IStatisticsResult
    {
        public NullStatisticsResult(double? value)
        {
            Value = value;
        }

        public void AddBucketLabels(int minimum, int step)
        {
        }

        public void AddData(double? data)
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

        public int WindowsSize => 0;

        public double? Value { get;  }

        public int TotalWords => 0;

        public int TotalOccurences => 0;

        public DescriptiveStatistics Statistics => null;

        public IEnumerable<string> BucketLabels => null;

        public IEnumerable<double> DataBag => null;

        public Histogram Histogram => null;

        public int Count => 0;

        public int BucketCount
        {
            get { return 0; }
            set { }
        }
    }
}
