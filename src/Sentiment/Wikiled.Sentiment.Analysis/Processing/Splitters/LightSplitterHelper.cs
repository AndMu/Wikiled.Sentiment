using System;
using NLog;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Analysis.Processing.Splitters
{
    public class LightSplitterHelper : ISplitterHelper
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public LightSplitterHelper()
        {
            Parallel = Environment.ProcessorCount / 2;
        }

        public IWordsHandler DataLoader { get; private set; }

        public ITextSplitter Splitter { get; private set; }

        public int Parallel { get; }

        public void Load()
        {
            log.Debug("Load");
            DataLoader = new BasicWordsHandler(new NaivePOSTagger(null, WordTypeResolver.Instance));
            Splitter = new SimpleTextSplitter(DataLoader);
        }
    }
}
