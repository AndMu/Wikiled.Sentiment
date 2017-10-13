using System.IO;
using NUnit.Framework;
using Wikiled.Core.Utility.Resources;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Dictionary;
using Wikiled.Text.Analysis.WordNet.Engine;

namespace Wikiled.Sentiment.Integration.Tests.Helpers
{
    public class DictionaryHelper
    {
        private DictionaryHelper()
        {
            var configuration = new ConfigurationHandler();
            var path = configuration.GetConfiguration("resources");
            path = Path.Combine(TestContext.CurrentContext.TestDirectory, path);
            configuration.SetConfiguration("resouces", path);
            LibraryPath = Path.Combine(path, @"Library\Standard\");
            Engine = new WordNetEngine(Path.Combine(path, @"Wordnet 3.0"));
            var dictionary = new BasicEnglishDictionary();
            WordsHandlers = new WordsDataLoader(LibraryPath, dictionary);
            WordsHandlers.Load();
        }

        public static DictionaryHelper Instance { get; } = new DictionaryHelper();

        public string LibraryPath { get; }

        public IWordsHandler WordsHandlers { get; }

        public IWordNetEngine Engine { get; }
    }
}
