using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Common.Logging;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Parser
{
    public class QueueTextSplitter : ITextSplitter
    {
        private readonly ILogger<QueueTextSplitter> log;

        private readonly ConcurrentBag<Lazy<ITextSplitter>> splitters = new ConcurrentBag<Lazy<ITextSplitter>>();

        private readonly ConcurrentQueue<Lazy<ITextSplitter>> workStack = new ConcurrentQueue<Lazy<ITextSplitter>>();

        private readonly SemaphoreSlim semaphore;

        public QueueTextSplitter(ILogger<QueueTextSplitter> log, int maxSplitters, Func<ITextSplitter> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (maxSplitters <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSplitters));
            }

            this.log = log ?? throw new ArgumentNullException(nameof(log));
            semaphore = new SemaphoreSlim(maxSplitters, maxSplitters);
            for (var i = 0; i < maxSplitters; i++)
            {
                var item = new Lazy<ITextSplitter>(factory);
                splitters.Add(item);
                workStack.Enqueue(item);
            }
        }

        public void Dispose()
        {
            log.LogDebug("Dispose");
            semaphore.Dispose();
            foreach (Lazy<ITextSplitter> splitter in splitters)
            {
                if (splitter.IsValueCreated)
                {
                    splitter.Value.Dispose();
                }
            }
        }

        public async Task<Document> Process(ParseRequest request)
        {
            await semaphore.WaitAsync().ConfigureAwait(false);
            if (!workStack.TryDequeue(out Lazy<ITextSplitter> splitter))
            {
                throw new InvalidOperationException("Synchronization error!");
            }

            try
            {
                return await splitter.Value.Process(request).ConfigureAwait(false);
            }
            finally
            {
                workStack.Enqueue(splitter);
                semaphore.Release();
            }
        }
    }
}
