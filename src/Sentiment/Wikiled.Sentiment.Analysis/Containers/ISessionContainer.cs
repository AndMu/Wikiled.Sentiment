using System;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Analysis.Containers
{
    public interface ISessionContainer : IDisposable
    {
        SessionContext Context { get; }

        ITextSplitter GetTextSplitter();

        IWordFactory GetWordFactory();

        ITestingClient GetTesting(string path = null);

        ITrainingClient GetTraining(string path);

        T Resolve<T>();
    }
}