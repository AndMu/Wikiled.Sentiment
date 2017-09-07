using Wikiled.Arff.Persistence;

namespace Wikiled.Sentiment.ConsoleApp.Machine.Data
{
    public class SemEvalData
    {
        public SemEvalData(long? id, PositivityType? original, string text)
        {
            Id = id;
            Original = original;
            Text = text;
        }

        public long? Id { get; }

        public double? Stars { get; set; }

        public PositivityType? CalculatedPositivity => !Stars.HasValue ? (PositivityType?)null : Stars > 3 ? PositivityType.Positive : PositivityType.Negative;

        public PositivityType? Original { get; }

        public string Text { get; }
    }
}
