using System;
using Microsoft.Extensions.DependencyInjection;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Analysis.Containers
{
    public sealed class SessionContainer : ISessionContainer
    {
        private readonly IServiceScope scope;

        public SessionContainer(IServiceProvider provider)
        {
            scope = provider?.CreateScope() ?? throw new ArgumentNullException(nameof(scope));
            Context = (SessionContext)scope.ServiceProvider.GetService<ISessionContext>();
        }

        public SessionContext Context { get; }

        public ITextSplitter GetTextSplitter()
        {
            return scope.ServiceProvider.GetService<ITextSplitter>();
        }

        public IWordFactory GetWordFactory()
        {
            return scope.ServiceProvider.GetService<IWordFactory>();
        }

        public ITestingClient GetTesting(string path = null)
        {
            Context.SvmPath = path;
            return scope.ServiceProvider.GetService<ITestingClient>();
        }

        public ITrainingClient GetTraining(string path)
        {
            Context.SvmPath = path;
            return scope.ServiceProvider.GetService<ITrainingClient>();
        }

        public T Resolve<T>()
        {
            return scope.ServiceProvider.GetService<T>();
        }

        public void Dispose()
        {
            scope?.Dispose();
        }
    }
}
