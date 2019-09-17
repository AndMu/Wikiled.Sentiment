using System;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Analysis.Containers
{
    public class SessionContainer : ISessionContainer
    {
        private readonly ILifetimeScope container;

        public SessionContainer(ILifetimeScope container)
        {
            this.container = container ?? throw new ArgumentNullException(nameof(container));
            Context = (SessionContext)container.Resolve<ISessionContext>();
        }

        public SessionContext Context { get; }

        public ITextSplitter GetTextSplitter()
        {
            return container.Resolve<ITextSplitter>();
        }

        public ITestingClient GetTesting(string path = null)
        {
            return container.Resolve<ITestingClient>(new NamedParameter("svmPath", path));
        }

        public ITrainingClient GetTraining(string path)
        {
            return container.Resolve<ITrainingClient>(new NamedParameter("svmPath", path));
        }

        public T Resolve<T>()
        {
            return container.Resolve<T>();
        }

        public void Dispose()
        {
            container?.Dispose();
        }
    }
}
