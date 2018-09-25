using System;
using Autofac;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Analysis.Containers
{
    public class ContainerHelper : IContainerHelper
    {
        public ContainerHelper(IContainer container)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public IContainer Container { get; }

        public SentimentContext Context => (SentimentContext)Container.Resolve<ISentimentContext>();

        public ITextSplitter GetTextSplitter()
        {
            return Container.Resolve<ITextSplitter>();
        }

        public IWordsHandler GetDataLoader()
        {
            return Container.Resolve<IWordsHandler>();
        }
    }
}
