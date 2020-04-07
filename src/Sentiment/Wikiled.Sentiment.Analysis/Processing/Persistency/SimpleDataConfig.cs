namespace Wikiled.Sentiment.Analysis.Processing.Persistency
{
    public class SimpleDataConfig : IDataSourceConfig
    {
        public string All { get; set; }

        public string Positive { get; set; }

        public string Negative { get; set; }
    }
}
