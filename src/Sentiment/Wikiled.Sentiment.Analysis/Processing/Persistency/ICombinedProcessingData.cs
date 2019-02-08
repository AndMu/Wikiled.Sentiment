using System;
using System.Reactive.Linq;

namespace Wikiled.Sentiment.Analysis.Processing.Persistency
{
    public class CombinedDataSource : IDataSource
    {
        private readonly IDataSource[] inner;

        public CombinedDataSource(IDataSource[] inner)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public IObservable<DataPair> Load()
        {
            var result = Observable.Empty<DataPair>();
            foreach (var processingData in inner)
            {
                result = result.Merge(processingData.Load());
            }

            return result;
        }
    }
}
