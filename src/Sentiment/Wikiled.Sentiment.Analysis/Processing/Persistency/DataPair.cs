using System;
using System.Threading.Tasks;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Processing.Persistency
{
    public class DataPair
    {
        public DataPair(SentimentClass? sentiment, Task<SingleProcessingData> data)
        {
            Sentiment = sentiment;
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public SentimentClass? Sentiment { get; }

        public Task<SingleProcessingData> Data { get; }
    }
}
