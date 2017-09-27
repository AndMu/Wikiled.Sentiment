namespace Wikiled.Sentiment.Analysis.Stats
{
    public class ResultRecord
    {
        public ResultRecord(string id, double expected, double? actual, int totalSentiments)
        {
            Id = id;
            Expected = expected;
            Actual = actual;
            TotalSentiments = totalSentiments;
        }

        public string Id { get; }

        public double Expected { get; }

        public double TotalSentiments { get; }

        public double? Actual { get; }

        public override string ToString()
        {
            var actual = Actual?.ToString() ?? string.Empty;
            return $"{Id},{Expected},{actual},{TotalSentiments}";
        }
    }
}
