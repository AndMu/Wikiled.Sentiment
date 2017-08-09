namespace Wikiled.Sentiment.Text.Data.Weighting
{
    public interface IItemCoefficient
    {
        IItemCoefficient Readjust(double value);
        string Text { get; }
        double Value { get; }
    }
}
