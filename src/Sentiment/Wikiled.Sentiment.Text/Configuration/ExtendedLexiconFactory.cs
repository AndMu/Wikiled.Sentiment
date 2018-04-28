using System;
using System.IO;
using NLog;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Text.Analysis.Dictionary;

namespace Wikiled.Sentiment.Text.Configuration
{
    public class ExtendedLexiconFactory : IExtendedLexiconFactory
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private Lazy<IWordsHandler> wordsHandler;

        public ExtendedLexiconFactory(IConfigurationHandler configuration)
        {
            Guard.NotNull(() => configuration, configuration);
            var path = configuration.ResolvePath("Resources");
            ResourcesPath = Path.Combine(path, configuration.SafeGetConfiguration("Lexicon", @"Library\Standard"));
        }

        public bool CanBeConstructed
        {
            get
            {
                if (!Directory.Exists(ResourcesPath))
                {
                    log.Error("Path doesn't exist: {0}", ResourcesPath);
                    return false;
                }

                return true;
            }
        }

        public bool CanConstruct => !IsConstructed && CanBeConstructed;

        public bool IsConstructed { get; private set; }

        public string ResourcesPath { get; }

        public IWordsHandler WordsHandler => wordsHandler.Value;

        public void Construct()
        {
            if (!CanBeConstructed)
            {
                throw new InvalidOperationException("Lexicon can't be constructed");
            }

            wordsHandler = new Lazy<IWordsHandler>(
                () =>
                    {
                        var handler = new WordsDataLoader(ResourcesPath, new BasicEnglishDictionary());
                        handler.Load();
                        return handler;
                    });

            log.Debug("Construct lexicon using path: {0}", ResourcesPath);
            IsConstructed = true;
        }
    }
}
