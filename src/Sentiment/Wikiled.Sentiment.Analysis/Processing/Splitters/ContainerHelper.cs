using System;
using Autofac;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Processing.Splitters
{
    public class ContainerHelper : IContainerHelper
    {
        public ContainerHelper(IContainer container)
        {            
            Container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public IContainer Container { get; }

        public ITextSplitter GetTextSplitter()
        {
            return Container.Resolve<ITextSplitter>();
        }

        public IWordsHandler GetDataLoader()
        {
            return Container.Resolve<IWordsHandler>();
        }

        public void ChangeWordsHandler(IAspectDectector detector)
        {
            throw new NotImplementedException();
        }

        public IParsedReviewManager Resolve(Document document)
        {
            throw new NotImplementedException();
        }
    }
}
