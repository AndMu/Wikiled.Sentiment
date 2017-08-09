using System;
using System.IO;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Text.Analysis.POS;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Parser.Cache;
using Wikiled.Text.Analysis.Cache;

namespace Wikiled.Sentiment.Text.NLP.Stanford
{
    public class StanfordSplitterFactory : ISplitterFactory
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly string resourcesPath;

        private readonly ILexiconFactory factory;

        private readonly ICacheFactory cachedFactory;

        public StanfordSplitterFactory(string path, ILexiconFactory factory, ICacheFactory cachedFactory)
        {
            Guard.NotNullOrEmpty(() => path, path);
            Guard.NotNull(() => factory, factory);
            Guard.NotNull(() => cachedFactory, cachedFactory);
            log.Debug("Creating with resource path: <{0}>", path);
            resourcesPath = path;
            this.factory = factory;
            this.cachedFactory = cachedFactory;
            this.cachedFactory = cachedFactory;
        }

        public bool CanBeConstructed
        {
            get
            {
                if (Directory.Exists(resourcesPath))
                {
                    return true;
                }

                log.Debug("Can't be constructed resources not found: {0}", resourcesPath);
                return false;
            }
        }

        public bool CanConstruct => CanBeConstructed && !IsConstructed;

        public bool IsConstructed { get; private set; }

        public ITextSplitter TextSplitter { get; private set; }

        public ITextSplitter ConstructSingle()
        {
            return new StanfordTextSplitter(resourcesPath, factory.WordsHandler, cachedFactory.Create(POSTaggerType.Stanford));
        }

        public void Construct()
        {
            if (!CanConstruct)
            {
                throw new InvalidOperationException($"Can't be constructed: {resourcesPath}");
            }

            log.Debug("Construct");
            TextSplitter = ConstructSingle();
            IsConstructed = true;
        }

        public void Dispose()
        {
            TextSplitter?.Dispose();
        }
    }
}