namespace Wikiled.Sentiment.Analysis.Processing.Persistency
{
    public interface IDataLoader
    {
        IDataSource Load(IDataSourceConfig souse);
    }
}