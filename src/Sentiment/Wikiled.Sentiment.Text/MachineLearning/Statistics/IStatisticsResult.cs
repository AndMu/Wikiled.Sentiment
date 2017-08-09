namespace Wikiled.Sentiment.Text.MachineLearning.Statistics
{
    public interface IStatisticsResult : ISimpleStatistics
    {
        void IncrementTotal();

        void IncrementOccurences();

        int WindowsSize { get; }

        double Value { get; }

        int TotalWords { get; }

        int TotalOccurences { get; }
    }
}