using System;
using System.Threading.Tasks;
using NLog;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.NLP
{
    public class RecyclableTextSplitter : ITextSplitter
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly ISplitterFactory factory;

        private readonly int maxProcessing;

        private int current;

        private ITextSplitter splitter;

        public RecyclableTextSplitter(ISplitterFactory factory, int maxProcessing = 1000)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.maxProcessing = maxProcessing;
        }

        public void Dispose()
        {
            splitter?.Dispose();
        }

        public async Task<Document> Process(ParseRequest request)
        {
            if (splitter == null)
            {
                log.Debug("Constructing NEW splitter...");
                current = 0;
                splitter = factory.ConstructSingle();
            }

            current++;
            var result = await splitter.Process(request);
            if (current >= maxProcessing)
            {
                splitter.Dispose();
                splitter = null;
            }

            return result;
        }
    }
}
