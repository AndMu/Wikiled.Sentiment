using System.IO;
using NUnit.Framework;
using Wikiled.Core.Utility.Resources;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.TestLogic.Shared.Helpers
{
    public class ActualWordsHandler
    {
        private ActualWordsHandler()
        {
            Configuration = new ConfigurationHandler();
            var resources = Configuration.GetConfiguration("Resources");
            var resourcesPath = Path.Combine(TestContext.CurrentContext.TestDirectory, resources);
            Configuration.SetConfiguration("Resources", resourcesPath);
            var factory = new ExtendedLexiconFactory(Configuration);
            factory.Construct();
            WordsHandler = factory.WordsHandler;
            ((WordsDataLoader)WordsHandler).Repair = null;
            TextSplitter = new SimpleTextSplitter(WordsHandler);
            Loader = new DocumentLoader(TextSplitter, WordsHandler);
        }

        public ConfigurationHandler Configuration { get; }

        public IWordsHandler WordsHandler { get; }

        public DocumentLoader Loader { get; }

        public ITextSplitter TextSplitter { get; } 

        public static ActualWordsHandler Instance { get; } = new ActualWordsHandler();
    }
}
