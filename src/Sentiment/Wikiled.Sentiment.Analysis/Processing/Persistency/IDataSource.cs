using System;

namespace Wikiled.Sentiment.Analysis.Processing.Persistency
{
    public interface IDataSource
    {
        IObservable<DataPair> Load();
    }
}