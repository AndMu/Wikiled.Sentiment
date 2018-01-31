using System.Collections.Generic;
using MathNet.Numerics.Statistics;

namespace Wikiled.Sentiment.Text.MachineLearning.Statistics
{
    public interface ISimpleStatistics
    {
        int BucketCount { get; set; }

        int Count { get; }

        IEnumerable<double> DataBag { get; }

        Histogram Histogram { get; }

        DescriptiveStatistics Statistics { get; }

        void AddData(double? data);
    }
}
