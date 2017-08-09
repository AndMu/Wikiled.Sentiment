using System;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Resources;
using Wikiled.Sentiment.Analysis.Workspace;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Text.Analysis.POS;
using Wikiled.Sentiment.Text.NLP.Stanford;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Parser.Cache;
using Wikiled.Text.Analysis.Cache;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class SplitterHelper : ISplitterHelper
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly ConfigurationHandler configuration;

        private readonly ICacheFactory cacheFactory;

        private bool isLoaded;

        private readonly IStanfordFactory stanford;

        public SplitterHelper(ICacheFactory cacheFactory, ConfigurationHandler configuration, IStanfordFactory stanford, int parallel = 0)
        {
            Guard.NotNull(() => cacheFactory, cacheFactory);
            Guard.NotNull(() => configuration, configuration);
            Guard.NotNull(() => stanford, stanford);
            Parallel = parallel <= 0 ? Environment.ProcessorCount / 2 : parallel;
            Parallel = Parallel <= 0 ? 1 : Parallel;
            log.Debug("Construct with parallel: {0}", Parallel);
            this.cacheFactory = cacheFactory;
            this.configuration = configuration;
            this.stanford = stanford;
        }

        public int Parallel { get; }

        public IWordsHandler DataLoader { get; private set; }

        public ITextSplitter Splitter { get; private set; }

        public void Load(POSTaggerType tagger = POSTaggerType.Stanford)
        {
            if (isLoaded)
            {
                return;
            }

            isLoaded = true;
            log.Info("Loading lexicon <{0}>...", tagger);
            var lexiconFactory = new ExtendedLexiconFactory(configuration);
            lexiconFactory.Construct();
            var textSplitter = new TextSplitterFactoryFinder(stanford, cacheFactory);
            var factory = textSplitter.Load(tagger, lexiconFactory, configuration);
            if (factory == null)
            {
                throw new InvalidOperationException("Failed to construct!!!");
            }

            factory.Construct();
            Splitter = new QueueTextSplitter(Parallel, factory);
            DataLoader = lexiconFactory.WordsHandler;
            var workspace = new WorkspaceInstance("Lexicon", ".", () => DataLoader);
            workspace.Init();
        }
    }
}
