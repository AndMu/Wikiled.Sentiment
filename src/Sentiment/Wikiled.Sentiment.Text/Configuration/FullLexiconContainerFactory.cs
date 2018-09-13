using System;
using System.IO;
using Autofac;
using NLog;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Text.Analysis.Dictionary;
using Wikiled.Text.Analysis.NLP.NRC;
using Wikiled.Text.Inquirer.Logic;

namespace Wikiled.Sentiment.Text.Configuration
{
    public class FullLexiconContainerFactory : ILexiconContainerFactory
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public FullLexiconContainerFactory(IConfigurationHandler configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var path = configuration.ResolvePath("Resources");
            var resourcesPath = Path.Combine(path, configuration.SafeGetConfiguration("Lexicon", @"Library/Standard"));
            if (!Directory.Exists(resourcesPath))
            {
                log.Error("Path doesn't exist: {0}", resourcesPath);
                throw new InvalidOperationException("Lexicon can't be constructed");
            }

            WordsHandler = new WordsDataLoader(resourcesPath);
        }
       
        public IWordsHandler WordsHandler { get; }
    }
}
