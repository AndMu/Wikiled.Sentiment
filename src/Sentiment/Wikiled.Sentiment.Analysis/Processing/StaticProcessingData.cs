using System;
using System.Linq;
using System.Reactive.Linq;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class StaticProcessingData : IProcessingData
    {
        public StaticProcessingData(ProcessingData data)
        {
            All = data.Neutral.Select(item => new DataPair(SentimentClass.Neutral, item)).ToObservable();
            All = All.Concat(data.Positive.Select(item => new DataPair(SentimentClass.Positive, item)).ToObservable());
            All = All.Concat(data.Negative.Select(item => new DataPair(SentimentClass.Negative, item)).ToObservable());
        }

        public IObservable<DataPair> All { get; }
    }
}
