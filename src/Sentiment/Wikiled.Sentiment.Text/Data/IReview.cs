namespace Wikiled.Sentiment.Text.Data
{
    public interface IReview
    {
        int ReviewId { get; }

        double? Stars { get; }
    }
}
