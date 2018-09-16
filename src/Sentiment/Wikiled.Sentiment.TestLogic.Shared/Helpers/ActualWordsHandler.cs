using System.IO;
using Autofac;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Processing.Splitters;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.TestLogic.Shared.Helpers
{
    public class ActualWordsHandler
    {
        private ActualWordsHandler(POSTaggerType type)
        {
            Configuration = new ConfigurationHandler();
            var resources = Configuration.GetConfiguration("Resources");
            var resourcesPath = Path.Combine(TestContext.CurrentContext.TestDirectory, resources);
            Configuration.SetConfiguration("Resources", resourcesPath);
            Container = new MainSplitterFactory(new NullCacheFactory(), Configuration) {SupportRepair = false}.Create(type);
            WordsHandler = Container.GetDataLoader();
            TextSplitter = Container.GetTextSplitter();
            WordFactory = Container.Container.Resolve<IWordFactory>();
            Loader = new DocumentLoader(Container);
        }

        public static ActualWordsHandler InstanceSimple { get; } = new ActualWordsHandler(POSTaggerType.Simple);

        public static ActualWordsHandler InstanceOpen { get; } = new ActualWordsHandler(POSTaggerType.SharpNLP);

        public IContainerHelper Container { get; }

        public IConfigurationHandler Configuration { get; }

        public IWordsHandler WordsHandler { get; }

        public DocumentLoader Loader { get; }

        public ITextSplitter TextSplitter { get; }

        public IWordFactory WordFactory { get; }
    }
}
