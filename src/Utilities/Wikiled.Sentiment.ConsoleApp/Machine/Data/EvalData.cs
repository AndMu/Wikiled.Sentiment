using Wikiled.Arff.Persistence;

namespace Wikiled.Sentiment.ConsoleApp.Machine.Data
{
    public class EvalData
    {
        public EvalData(string id, PositivityType? original, string text)
        {
            Id = id;
            Original = original;
            Text = text;
        }

        public string Id { get; }

        public double? Stars { get; set; }

        public PositivityType? CalculatedPositivity => !Stars.HasValue ? (PositivityType?)null : Stars > 3 ? PositivityType.Positive : PositivityType.Negative;

        public PositivityType? Original { get; }

        public string Text { get; }
    }
}
