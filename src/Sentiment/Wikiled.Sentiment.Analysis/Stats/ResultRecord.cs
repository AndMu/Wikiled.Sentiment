using System;

namespace Wikiled.Sentiment.Analysis.Stats
{
    public class ResultRecord
    {
        public ResultRecord(string id, double? expected, double? actual, int totalSentiments, DateTime? date)
        {
            Id = id;
            Expected = expected;
            Actual = actual;
            TotalSentiments = totalSentiments;
            Date = date;
        }

        public string Id { get; }

        public DateTime? Date { get; set; }

        public double? Expected { get; }

        public double TotalSentiments { get; }

        public double? Actual { get; }
    }
}
