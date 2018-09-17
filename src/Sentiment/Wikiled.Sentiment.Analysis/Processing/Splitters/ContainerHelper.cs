using System;
using Autofac;
using Autofac.Core;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Processing.Splitters
{
    public class ContainerHelper : IContainerHelper
    {
        public ContainerHelper(IContainer container, SentimentContext context)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IContainer Container { get; }

        public SentimentContext Context { get; }

        public ITextSplitter GetTextSplitter()
        {
            return Container.Resolve<ITextSplitter>();
        }

        public IWordsHandler GetDataLoader()
        {
            return Container.Resolve<IWordsHandler>();
        }

        public IParsedReviewManager Resolve(Document document)
        {
            return Container.Resolve<IParsedReviewManager>(new NamedParameter("document", document));
        }
    }
}
