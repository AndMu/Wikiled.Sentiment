using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public class SentimentValue
    {
        public SentimentValue(IWordItem sentiment, string span, SentimentValueData value)
            : this(sentiment, span)
        {
            DataValue = value;
        }

        public SentimentValue(IWordItem sentiment, string span, double value)
            : this(sentiment, span, new SentimentValueData(value))
        {
        }

        private SentimentValue(IWordItem sentiment, string span)
        {
            Owner = sentiment;
            Span = span;
        }

        public string Span { get; }

        public SentimentValueData DataValue { get; }

        public IWordItem Owner { get; }

        public static SentimentValue CreateBad(IWordItem sentiment)
        {
            return new SentimentValue(sentiment, sentiment.Text, -1);
        }

        public static SentimentValue CreateGood(IWordItem sentiment)
        {
            return new SentimentValue(sentiment, sentiment.Text, 1);
        }

        public static SentimentValue CreateInvertor(IWordItem sentiment)
        {
            return new SentimentValue(sentiment, sentiment.Text, -1.5);
        }

        public SentimentValue GetDistanced(int distance)
        {
            return distance > 2
                       ? new SentimentValue(Owner, Span, new SentimentValueData(DataValue.Value / (distance - 1), DataValue.SentimentSource))
                       : new SentimentValue(Owner, Span, DataValue);
        }

        public override string ToString()
        {
            return $"Sentiment: [{Owner}] [{DataValue}]";
        }
    }
}
