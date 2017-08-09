using System;
using System.IO;
using System.Runtime.Caching;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Cache;
using Wikiled.Core.Utility.Resources;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.WordNet.InformationContent;
using Wikiled.Text.Analysis.Dictionary;
using Wikiled.Text.Analysis.WordNet.Engine;

namespace Wikiled.Sentiment.Text.Configuration
{
    public class ExtendedLexiconFactory : IExtendedLexiconFactory
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly string path;

        private Lazy<IInformationContentResnik> resnik;

        private Lazy<IWordNetEngine> wordNet;

        private Lazy<IWordsHandler> wordsHandler;

        private Lazy<IRelatednessMesaure> relatedness;

        public ExtendedLexiconFactory(IConfigurationHandler configuration)
        {
            Guard.NotNull(() => configuration, configuration);
            path = configuration.ResolvePath("Resources");
            ResourcesPath = Path.Combine(path, configuration.SafeGetConfiguration("Lexicon", @"Library\Standard"));
        }   

        public void Construct()
        {
            if (!CanBeConstructed)
            {
                throw new InvalidOperationException("Lexicon can't be constructed");
            }

            log.Debug("Construct lexicon using path: {0}", ResourcesPath);
            resnik = new Lazy<IInformationContentResnik>(() =>
            {
                var info = new DirectoryInfo(Path.Combine(WordNetInfoContentPath, "ic-brown-resnik-add1.dat"));
                return InformationContentResnik.Load(info.FullName);
            });

            wordNet = new Lazy<IWordNetEngine>(() =>
            {
                var info = new DirectoryInfo(WordNetPath);
                return new WordNetEngine(info.FullName);
            });

            wordsHandler = new Lazy<IWordsHandler>(() =>
            {
                var handler = new WordsDataLoader(ResourcesPath, new EnglishDictionary(ResourcesPath, WordNetEngine));
                handler.Load();
                return handler;
            });

            relatedness = new Lazy<IRelatednessMesaure>(() => new JcnMeasure(new RuntimeCache(new MemoryCache("Data"), TimeSpan.FromMinutes(1)), Resnik, WordNetEngine));
            IsConstructed = true;
        }

        public bool CanBeConstructed
        {
            get
            {
                if (!Directory.Exists(ResourcesPath))
                {
                    log.Debug("Path doesn't exist: {0}", ResourcesPath);
                    return false;
                }

                if (!Directory.Exists(WordNetPath))
                {
                    log.Debug("Path doesn't exist: {0}", WordNetPath);
                    return false;
                }

                if (!Directory.Exists(WordNetInfoContentPath))
                {
                    log.Debug("Path doesn't exist: {0}", WordNetInfoContentPath);
                    return false;
                }

                return true;
            }
        }

        public bool CanConstruct => !IsConstructed && CanBeConstructed;

        public bool IsConstructed { get; private set; }

        public string WordNetPath => Path.Combine(path, @"Wordnet 3.0");

        public string WordNetInfoContentPath => Path.Combine(path, @"WordNet-InfoContent-3.0");

        public string ResourcesPath { get; }

        public IRelatednessMesaure RelatednessMesaure => relatedness.Value;

        public IWordsHandler WordsHandler => wordsHandler.Value;

        public IInformationContentResnik Resnik => resnik.Value;

        public IWordNetEngine WordNetEngine => wordNet.Value;
    }
}
