using Autofac;
using NUnit.Framework;
using System.IO;
using Wikiled.Sentiment.Analysis.Containers;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.TestLogic.Shared.Helpers
{
    public class ActualWordsHandler
    {
        public ActualWordsHandler(POSTaggerType type, bool supportRepair = false)
        {
            var factory = MainContainerFactory.Setup()
                .SetupRepair(supportRepair)
                .WithContext()
                .Config(configuration =>
                {
                    string resources = configuration.GetConfiguration("Resources");
                    string resourcesPath = Path.Combine(TestContext.CurrentContext.TestDirectory, resources);
                    configuration.SetConfiguration("Resources", resourcesPath);
                })
                .Splitter(type)
                .SetupNullCache();

            Container = factory.Create();
            WordsHandler = Container.GetDataLoader();
            TextSplitter = Container.GetTextSplitter();
            WordFactory = Container.Container.Resolve<IWordFactory>();
            Loader = new DocumentLoader(Container);
        }

        public void Reset()
        {
            Container.Context.Reset();
        }

        public static ActualWordsHandler InstanceSimple { get; } = new ActualWordsHandler(POSTaggerType.Simple);

        public static ActualWordsHandler InstanceOpen { get; } = new ActualWordsHandler(POSTaggerType.SharpNLP);

        public IContainerHelper Container { get; }

        public IConfigurationHandler Configuration => Container.Container.Resolve<IConfigurationHandler>();

        public IWordsHandler WordsHandler { get; }

        public DocumentLoader Loader { get; }

        public ITextSplitter TextSplitter { get; }

        public IWordFactory WordFactory { get; }
    }
}
