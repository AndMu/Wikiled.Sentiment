using System;
using System.IO;
using NLog;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Resources;

namespace Wikiled.Sentiment.Text.Configuration
{
    public class FullLexiconContainerFactory : ILexiconContainerFactory
    {
        private readonly IConfigurationHandler configuration;

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public FullLexiconContainerFactory(IConfigurationHandler configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
       
        public IWordsHandler Construct()
        {
            var path = configuration.ResolvePath("Resources");
            var resourcesPath = Path.Combine(path, configuration.SafeGetConfiguration("Lexicon", @"Library/Standard"));
            if (!Directory.Exists(resourcesPath))
            {
                log.Error("Path doesn't exist: {0}", resourcesPath);
                throw new InvalidOperationException("Lexicon can't be constructed");
            }

            return new WordsDataLoader(resourcesPath);
        }
    }
}
