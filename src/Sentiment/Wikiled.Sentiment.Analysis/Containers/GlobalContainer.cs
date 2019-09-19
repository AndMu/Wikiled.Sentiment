using System;
using Microsoft.Extensions.DependencyInjection;

namespace Wikiled.Sentiment.Analysis.Containers
{
    public class GlobalContainer : IGlobalContainer
    {
        private readonly IServiceProvider container;

        public GlobalContainer(IServiceProvider container)
        {
            this.container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public ISessionContainer StartSession()
        {
            return new SessionContainer(container);
        }
    }
}
