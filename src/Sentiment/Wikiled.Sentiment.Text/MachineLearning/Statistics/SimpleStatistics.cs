using System.Collections.Concurrent;
using System.Collections.Generic;
using MathNet.Numerics.Statistics;

namespace Wikiled.Sentiment.Text.MachineLearning.Statistics
{
    public class SimpleStatistics : ISimpleStatistics
    {
        private Histogram histogram;
        private readonly ConcurrentBag<double> dataBag = new ConcurrentBag<double>();
        private DescriptiveStatistics statistics;

        public SimpleStatistics(params double[] dataPoints)
        {
            BucketCount = 10;
            if (dataPoints.Length > 0)
            {
                dataBag = new ConcurrentBag<double>(dataPoints);
            }
        }

        public void AddBucketData(int step, int total)
        {
            for (int i = 0; i < total; i++)
            {
                AddData(step);
            }
        }

        public void AddData(double data)
        {
            if (data == 0)
            {
                return;
            }

            dataBag.Add(data);
            statistics = null;
            histogram = null;
        }

        public Histogram Histogram
        {
            get
            {
                if (Count == 0)
                {
                    return null;
                }

                return histogram ?? (histogram = new Histogram(DataBag, BucketCount));
            }
        }

        public DescriptiveStatistics Statistics
        {
            get
            {
                if (Count == 0)
                {
                    return null;
                }

                return statistics ?? (statistics = new DescriptiveStatistics(DataBag));
            }
        }

        public IEnumerable<double> DataBag => dataBag;

        public int Count => dataBag.Count;

        public int BucketCount { get; set; }
    }
}
