using System.IO;
using NUnit.Framework;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Text.Analysis.Dictionary;

namespace Wikiled.Sentiment.Integration.Tests.Helpers
{
    public class DictionaryHelper
    {
        public DictionaryHelper()
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

        public static DictionaryHelper Default { get; } = new DictionaryHelper();

        public string LibraryPath { get; }

        public IWordsHandler WordsHandlers { get; }
    }
}
