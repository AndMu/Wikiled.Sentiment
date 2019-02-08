using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Wikiled.Sentiment.Analysis.Processing.Persistency
{
    public class StaticDataSource : IDataSource
    {
        private readonly ProcessingData data;

        public StaticDataSource(ProcessingData data)
        {
            this.data = data;
        }

        public IObservable<DataPair> Load()
        {
            var observable = data.Neutral.Select(item => new DataPair(SentimentClass.Neutral, Task.FromResult(item))).ToObservable();
            observable = observable.Concat(data.Positive.Select(item => new DataPair(SentimentClass.Positive, Task.FromResult(item))).ToObservable());
            observable = observable.Concat(data.Negative.Select(item => new DataPair(SentimentClass.Negative, Task.FromResult(item))).ToObservable());
            return observable;
        }
    }
}
