using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Wikiled.Sentiment.Analysis.Containers;
using Wikiled.Sentiment.Text.NLP.NER;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.TestLogic.Shared.Helpers
{
    public class ActualWordsHandler
    {
        private readonly IGlobalContainer container;

        public ActualWordsHandler(POSTaggerType type)
        {
            var factory = MainContainerFactory.Setup(new ServiceCollection())
                .Config()
                .Splitter(type, true)
                .AddNER(
                    new DictionaryNERResolver(
                        new Dictionary<string, string>
                        {
                            { "XXX", "XXX" }
                        }))
                .SetupNullCache();

            container = factory.Create();
            Container = container.StartSession();
            Loader = new DocumentLoader(Container);
        }

        public void Reset()
        {
            Container?.Dispose();
            Container = container.StartSession();
        }

        public static ActualWordsHandler InstanceSimple { get; } = new ActualWordsHandler(POSTaggerType.Simple);

        public static ActualWordsHandler InstanceOpen { get; } = new ActualWordsHandler(POSTaggerType.SharpNLP);

        public ISessionContainer Container { get; private set; }

        public IContextWordsHandler WordsHandler => Container.Resolve<IContextWordsHandler>();

        public DocumentLoader Loader { get; }

        public ITextSplitter TextSplitter => Container.Resolve<ITextSplitter>();

        public IWordFactory WordFactory => Container.Resolve<IWordFactory>();
    }
}
