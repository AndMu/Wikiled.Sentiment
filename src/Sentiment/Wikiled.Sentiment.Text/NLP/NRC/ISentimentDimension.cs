namespace Wikiled.Sentiment.Text.NLP.NRC
{
    public interface ISentimentDimension
    {
        int Anger { get; }

        int Anticipation { get; }

        int Disgust { get; }

        int Fear { get; }

        int Joy { get; }

        int Sadness { get; }

        int Surprise { get; }

        int Trust { get; }
    }
}