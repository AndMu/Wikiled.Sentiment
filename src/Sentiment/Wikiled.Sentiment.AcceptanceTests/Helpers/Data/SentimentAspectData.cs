namespace Wikiled.Sentiment.AcceptanceTests.Helpers.Data
{
    public class SentimentAspectData
    {
        public SentimentAspectData(SentimentTestData sentiment, TopItems attributes, TopItems features)
        {
            Sentiment = sentiment;
            Features = features;
            Attributes = attributes;
        }

        public SentimentTestData Sentiment { get; }

        public TopItems Features { get; }

        public TopItems Attributes { get; }

        public override string ToString()
        {
            return $"{Sentiment} Attributes: {Attributes} Features: {Features}";
        }
    }
}
