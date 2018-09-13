using System;
using System.IO;
using Autofac;
using NLog;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Text.NLP.OpenNLP
{
    public class OpenNlpSplitterFactory : ISplitterFactory
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly string resourcesPath;

        private readonly IWordsHandler wordsHandler;

        private readonly ICacheFactory cachedFactory;

        public OpenNlpSplitterFactory(string path, ILexiconContainerFactory containerFactory, ICacheFactory cachedFactory)
        {
            if (containerFactory is null)
            {
                throw new ArgumentNullException(nameof(containerFactory));
            }

            if (cachedFactory is null)
            {
                throw new ArgumentNullException(nameof(cachedFactory));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(path));
            }

            log.Debug("Creating with resource path: {0}", path);
            resourcesPath = path;
            this.cachedFactory = cachedFactory;
            wordsHandler = containerFactory.Construct();
        }

        public bool CanConstruct => Directory.Exists(resourcesPath) && !IsConstructed;

        public bool IsConstructed { get; private set; }

        public ITextSplitter TextSplitter { get; private set; }

        public ITextSplitter ConstructSingle()
        {
            return new OpenNLPTextSplitter(wordsHandler, resourcesPath, cachedFactory.Create(POSTaggerType.SharpNLP));
        }

        public void Construct()
        {
            if (!CanConstruct)
            {
                throw new InvalidOperationException("Can't be constructed");
            }

            TextSplitter = ConstructSingle();
            IsConstructed = true;
        }

        public void Dispose()
        {
            TextSplitter?.Dispose();
        }
    }
}