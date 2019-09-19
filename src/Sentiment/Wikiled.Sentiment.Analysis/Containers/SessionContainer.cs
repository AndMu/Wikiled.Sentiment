using System;
using Microsoft.Extensions.DependencyInjection;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Analysis.Containers
{
    public class SessionContainer : ISessionContainer
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

        public ITestingClient GetTesting(string path = null)
        {
            return scope.ServiceProvider.GetService<Func<string, ITestingClient>>()(path);
        }

        public ITrainingClient GetTraining(string path)
        {
            return scope.ServiceProvider.GetService<Func<string, ITrainingClient>>()(path);
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
