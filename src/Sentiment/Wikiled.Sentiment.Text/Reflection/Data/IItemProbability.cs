namespace Wikiled.Sentiment.Text.Reflection.Data
{
    public interface IItemProbability<out T>
    {
        T Data { get; }

        double Probability { get; set; }
    }
}