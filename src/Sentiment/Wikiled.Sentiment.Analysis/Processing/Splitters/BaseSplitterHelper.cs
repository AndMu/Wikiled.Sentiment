using System;
using NLog;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Resources;

namespace Wikiled.Sentiment.Analysis.Processing.Splitters
{
    public abstract class BaseSplitterHelper : ISplitterHelper
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IConfigurationHandler configuration;

        private bool isLoaded;

        public BaseSplitterHelper(IConfigurationHandler configuration, int parallel = 0)
        {            
            Guard.NotNull(() => configuration, configuration);
            Parallel = parallel <= 0 ? Environment.ProcessorCount / 2 : parallel;
            Parallel = Parallel <= 0 ? 1 : Parallel;
            log.Debug("Construct with parallel: {0}", Parallel);
            this.configuration = configuration;
        }

        public int Parallel { get; }

        public IWordsHandler DataLoader { get; private set; }

        public ITextSplitter Splitter { get; private set; }

        public void Load()
        {
            if (isLoaded)
            {
                return;
            }

            isLoaded = true;
            log.Info("Loading lexicon...");
            var lexiconFactory = new ExtendedLexiconFactory(configuration);
            lexiconFactory.Construct();
            var factory = Construct(lexiconFactory);
            if (factory == null)
            {
                throw new InvalidOperationException("Failed to construct!!!");
            }

            factory.Construct();
            Splitter = new QueueTextSplitter(Parallel, factory);
            DataLoader = lexiconFactory.WordsHandler;
        }

        protected abstract ISplitterFactory Construct(ILexiconFactory lexiconFactory);
    }
}
