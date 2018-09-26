using System;
using Autofac;

namespace Wikiled.Sentiment.Analysis.Containers
{
    public class GlobalContainer : IGlobalContainer
    {
        private readonly IContainer container;

        public GlobalContainer(IContainer container)
        {
            this.container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public ISessionContainer StartSession()
        {
            return new SessionContainer(container.BeginLifetimeScope());
        }
    }
}
