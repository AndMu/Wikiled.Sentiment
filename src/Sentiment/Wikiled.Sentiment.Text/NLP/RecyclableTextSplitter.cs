using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.NLP
{
    public class RecyclableTextSplitter : ITextSplitter
    {
        private readonly ISplitterFactory factory;

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly int maxProcessing;

        private static int total;

        private int current;

        private readonly int id;

        private ITextSplitter splitter;

        public RecyclableTextSplitter(ISplitterFactory factory, int maxProcessing = 5000)
        {
            id = Interlocked.Increment(ref total);
            id = total;
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.maxProcessing = maxProcessing + new Random().Next(1000);
        }

        public void Dispose()
        {
            splitter?.Dispose();
        }

        public async Task<Document> Process(ParseRequest request)
        {
            if (splitter == null)
            {
                log.Info("Constructing NEW {0} splitter...", id);
                Interlocked.Exchange(ref current, 0);
                splitter = factory.ConstructSingle();
            }

            var result = await splitter.Process(request).ConfigureAwait(false);
            if (Interlocked.Increment(ref current) >= maxProcessing)
            {
                splitter.Dispose();
                splitter = null;
            }

            return result;
        }
    }
}
