using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.MachineLearning
{
    public class TrainedSentimentProvider : ISentimentProvider
    {
        private readonly ISentimentProvider underlying;

        private readonly ITrainingPerspective perspective;

        public TrainedSentimentProvider(ISentimentProvider underlying, ITrainingPerspective perspective)
        {
            Guard.NotNull(() => underlying, underlying);
            Guard.NotNull(() => perspective, perspective);
            this.underlying = underlying;
            this.perspective = perspective;
        }

        public SentimentValue MeasureSentiment(IWordItem word)
        {
            var existing = underlying.MeasureSentiment(word)?.DataValue.Value ?? 1;
            var cells = new[] { new TextVectorCell(word, existing) };
            var vector = perspective.MachineSentiment.GetVector(cells, perspective.TrainingHeader.Normalization);
            var result = perspective.MachineSentiment.CalculateRating(vector);
            return result.Result.Probability.HasValue ? new SentimentValue(word, result.Result.Probability.Value) : null;
        }
    }
}
