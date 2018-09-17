using Autofac;
using NUnit.Framework;
using System.IO;
using Wikiled.Sentiment.Analysis.Processing.Splitters;
using Wikiled.Sentiment.Text.Configuration;
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
            string resources = Configuration.GetConfiguration("Resources");
            string resourcesPath = Path.Combine(TestContext.CurrentContext.TestDirectory, resources);
            Configuration.SetConfiguration("Resources", resourcesPath);

            Context = new SentimentContext();
            Container = new MainSplitterFactory(new NullCacheFactory(), Configuration) { SupportRepair = false }.Create(type, Context);
            WordsHandler = Container.GetDataLoader();
            TextSplitter = Container.GetTextSplitter();
            WordFactory = Container.Container.Resolve<IWordFactory>();
            Loader = new DocumentLoader(Container);
        }

        public void Reset()
        {
            Context.DisableFeatureSentiment = false;
            Context.DisableInvertors = false;
            Context.ChangeAspect(null);
        }

        public SentimentContext Context { get; }

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
