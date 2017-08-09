using System.Collections.Generic;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Aspects
{
    public interface IMainAspectHandler
    {
        void Process(IParsedReview review);

        IEnumerable<IWordItem> GetFeatures(int total);

        IEnumerable<IWordItem> GetAttributes(int total);
    }
}