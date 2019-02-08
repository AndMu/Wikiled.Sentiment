using Wikiled.Sentiment.Analysis.Processing.Persistency;

namespace Wikiled.Sentiment.Analysis.Tests.Processing.Persistency
{
    public class SimpleDataSource : IDataSourceConfig
    {
        public string All { get; set; }

        public string Positive { get; set; }

        public string Negative { get; set; }
    }
}
