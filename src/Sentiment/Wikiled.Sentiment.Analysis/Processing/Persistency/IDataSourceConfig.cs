namespace Wikiled.Sentiment.Analysis.Processing.Persistency
{
    public interface IDataSourceConfig
    {
        string All { get; } 
         
        string Positive { get; }

        string Negative { get; }
    }
}
