using System;
using System.Collections.Generic;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Text.Resources
{
    public class CachedSentimentDataReader : ISentimentDataReader
    {
        private readonly Lazy<IEnumerable<WordSentimentValueData>> cached;

        public CachedSentimentDataReader(ISentimentDataReader inner)
        {
            Guard.NotNull(() => inner, inner);
            cached = new Lazy<IEnumerable<WordSentimentValueData>>(inner.Read);
        }

        public IEnumerable<WordSentimentValueData> Read()
        {
            return cached.Value;
        }
    }
}
