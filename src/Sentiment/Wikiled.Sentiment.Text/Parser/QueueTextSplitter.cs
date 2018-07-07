using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Parser
{
    public class QueueTextSplitter : ITextSplitter
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentBag<Lazy<ITextSplitter>> splitters = new ConcurrentBag<Lazy<ITextSplitter>>();

        private readonly ConcurrentStack<Lazy<ITextSplitter>> workStack = new ConcurrentStack<Lazy<ITextSplitter>>();

        private readonly SemaphoreSlim semaphore;

        public QueueTextSplitter(int maxSplitters, ISplitterFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (maxSplitters <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSplitters));
            }

            semaphore = new SemaphoreSlim(maxSplitters, maxSplitters);
            for (int i = 0; i < maxSplitters; i++)
            {
                var item = new Lazy<ITextSplitter>(factory.ConstructSingle);
                splitters.Add(item);
                workStack.Push(item);
            }
        }

        public void Dispose()
        {
            log.Debug("Dispose");
            semaphore.Dispose();
            foreach (var splitter in splitters)
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
            if (!workStack.TryPop(out var splitter))
            {
                throw new InvalidOperationException("Synchronization error!");
            }

            try
            {
                return await splitter.Value.Process(request).ConfigureAwait(false);
            }
            finally
            {
                workStack.Push(splitter);
                semaphore.Release();
            }
        }
    }
}
