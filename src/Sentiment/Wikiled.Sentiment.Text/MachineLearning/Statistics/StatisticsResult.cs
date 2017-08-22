using System.Collections.Generic;
using System.Threading;
using MathNet.Numerics.Statistics;

namespace Wikiled.Sentiment.Text.MachineLearning.Statistics
{
    public class StatisticsResult : IStatisticsResult
    {
        private int totalWords;
        private int totalOccurences;
        private readonly SimpleStatistics statistics;

        public StatisticsResult(double value, int windowsSize, params double[] dataPoints)
        {
            Value = value;
            WindowsSize = windowsSize;
            statistics = new SimpleStatistics(dataPoints);
        }

        public void AddData(double data)
        {
            statistics.AddData(data);
        }

        public void AddBucketData(int step, int total)
        {
            statistics.AddBucketData(step, total);
        }

        public Histogram Histogram
        {
            get { return statistics.Histogram; }
        }

        public void IncrementTotal()
        {
            Interlocked.Increment(ref totalWords);
        }

        public void IncrementOccurences()
        {
            Interlocked.Increment(ref totalOccurences);
        }

        public IEnumerable<double> DataBag
        {
            get { return statistics.DataBag; }
        }

        public string XAxisTitle { get; set; }

        public int Count
        {
            get { return statistics.Count; }
        }

        public int BucketCount
        {
            get { return statistics.BucketCount; }
            set { statistics.BucketCount = value; }
        }

        public int WindowsSize { get;  }

        public double Value { get;  }

        public int TotalWords
        {
            get { return totalWords; }
        }

        public int TotalOccurences
        {
            get { return totalOccurences; }
        }

        public DescriptiveStatistics Statistics
        {
            get { return statistics.Statistics; }
        }
    }
}
