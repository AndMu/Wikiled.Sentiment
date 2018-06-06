using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class DataPair
    {
        public DataPair(SentimentClass sentiment, SingleProcessingData data)
        {
            Guard.NotNull(() => data, data);
            Sentiment = sentiment;
            Data = data;
        }

        public SentimentClass Sentiment { get; }

        public SingleProcessingData Data { get; }
    }
}
