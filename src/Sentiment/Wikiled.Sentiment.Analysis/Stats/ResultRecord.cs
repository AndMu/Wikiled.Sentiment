namespace Wikiled.Sentiment.Analysis.Stats
{
    public class ResultRecord
    {
        public ResultRecord(string id, double expected, double? actual)
        {
            Id = id;
            Expected = expected;
            Actual = actual;
        }

        public string Id { get; }

        public double Expected { get; }

        public double? Actual { get; }

        public override string ToString()
        {
            var actual = Actual?.ToString() ?? string.Empty;
            return $"{Id},{Expected},{actual}";
        }
    }
}
