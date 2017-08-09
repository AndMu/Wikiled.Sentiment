namespace Wikiled.Sentiment.AcceptanceTests.Helpers.Data
{
    public class TopAspectData
    {
        public TopAspectData(double total, string name, double sentiment)
        {
            Total = total;
            Name = name;
            Sentiment = sentiment;
        }

        public double Total { get; private set; }

        public string Name { get; private set; }

        public double Sentiment { get; private set; }
    }
}
