using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public class SentimentValue
    {
        public SentimentValue(IWordItem sentiment, SentimentValueData value)
            : this(sentiment)
        {
            DataValue = value;
        }

        public SentimentValue(IWordItem sentiment, double value)
            : this(sentiment, new SentimentValueData(value))
        {
        }

        private SentimentValue(IWordItem sentiment)
        {
            Owner = sentiment;
        }

        public SentimentValueData DataValue { get; }

        public IWordItem Owner { get; }

        public static SentimentValue CreateBad(IWordItem sentiment)
        {
            return new SentimentValue(sentiment, -1);
        }

        public static SentimentValue CreateGood(IWordItem sentiment)
        {
            return new SentimentValue(sentiment, 1);
        }

        public static SentimentValue CreateInvertor(IWordItem sentiment)
        {
            return new SentimentValue(sentiment, -1.5);
        }

        public SentimentValue GetDistanced(int distance)
        {
            return distance > 2
                       ? new SentimentValue(Owner, new SentimentValueData(DataValue.Value / (distance - 1), DataValue.SentimentSource))
                       : new SentimentValue(Owner, DataValue);
        }

        public override string ToString()
        {
            return $"Sentiment: [{Owner}] [{DataValue}]";
        }
    }
}
