using Wikiled.Arff.Logic;

namespace Wikiled.Sentiment.ConsoleApp.Analysis
{
    namespace Wikiled.Sentiment.ConsoleApp.Extraction.Bootstrap.Data
    {
        public class EvalData
        {
            public EvalData(string id, string text)
            {
                Id = id;
                Text = text;
            }

            public string Id { get; }

            public double? Stars { get; set; }

            public int TotalSentiments { get; set; }

            public bool? IsNeutral { get; set; }

            public PositivityType? CalculatedPositivity => IsNeutral == true ? PositivityType.Neutral : !Stars.HasValue ? (PositivityType?)null : Stars > 3 ? PositivityType.Positive : PositivityType.Negative;

            public string Text { get; }
        }
    }
}
