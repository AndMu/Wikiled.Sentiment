using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class DataPair
    {
        public DataPair(SentimentClass sentiment, SingleProcessingData data)
        {
            Sentiment = sentiment;
            Data = data ?? throw new System.ArgumentNullException(nameof(data));
        }

        public SentimentClass Sentiment { get; }

        public SingleProcessingData Data { get; }
    }
}
