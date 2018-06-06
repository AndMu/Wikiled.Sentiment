using System;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public interface IProcessingData
    {
        IObservable<DataPair> All { get; }
    }
}