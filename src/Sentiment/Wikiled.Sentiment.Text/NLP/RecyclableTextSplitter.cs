using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.NLP
{
    public sealed class RecyclableTextSplitter : ITextSplitter
    {
        private readonly ILogger<RecyclableTextSplitter> log;

        private readonly int maxProcessing;

        private static int total;

        private int current;

        private readonly int id;

        private ITextSplitter splitter;

        private readonly Func<ITextSplitter> factory;

        public RecyclableTextSplitter(ILogger<RecyclableTextSplitter> log, Func<ITextSplitter> factory, RecyclableConfig config)
        {
            id = Interlocked.Increment(ref total);
            id = total;
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.log = log;
            maxProcessing = config.MaxProcessing + new Random().Next(1000);
            log.LogDebug("Creating with maxProcessing:{0}", maxProcessing);
        }

        public void Dispose()
        {
            splitter?.Dispose();
        }

        public async Task<Document> Process(ParseRequest request)
        {
            if (splitter == null)
            {
                log.LogInformation("Constructing NEW {0} splitter...", id);
                Interlocked.Exchange(ref current, 0);
                splitter = factory();
            }

            Document result = await splitter.Process(request).ConfigureAwait(false);
            if (Interlocked.Increment(ref current) >= maxProcessing)
            {
                splitter.Dispose();
                splitter = null;
            }

            return result;
        }
    }
}
