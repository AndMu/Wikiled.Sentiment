using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Processing.Containers
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

        public IParsedReviewManager Resolve(Document document, ISentimentDataHolder lexicon = null)
        {
            List<Parameter> parameters = new List<Parameter>();
            parameters.Add(new NamedParameter("document", document));
            if (lexicon != null)
            {
                var loader = new CustomWordsDataLoader(GetDataLoader(), lexicon);
                parameters.Add(new NamedParameter("manager", loader));
                var wordFactory = Container.Resolve<IWordFactory>(new NamedParameter("wordsHandlers", loader));
                parameters.Add(new NamedParameter("wordsFactory", wordFactory));
            }

            return Container.Resolve<IParsedReviewManager>(parameters);
        }
    }
}
