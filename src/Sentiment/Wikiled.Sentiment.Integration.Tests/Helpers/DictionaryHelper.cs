using System.IO;
using NUnit.Framework;
using Wikiled.Common.Resources;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Dictionary;

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
            var dictionary = new BasicEnglishDictionary();
            WordsHandlers = new WordsDataLoader(LibraryPath, dictionary);
            WordsHandlers.Load();
        }

        public static DictionaryHelper Instance { get; } = new DictionaryHelper();

        public string LibraryPath { get; }

        public IWordsHandler WordsHandlers { get; }
    }
}
