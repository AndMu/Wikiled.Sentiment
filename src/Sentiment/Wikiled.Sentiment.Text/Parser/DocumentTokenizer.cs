using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Helpers;
using Wikiled.Core.Utility.Logging;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Extensions;

namespace Wikiled.Sentiment.Text.Parser
{
    public class DocumentTokenizer
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly ITextSplitter textSplitter;

        private int current;

        public DocumentTokenizer(ITextSplitter textSplitter)
        {
            Guard.NotNull(() => textSplitter, textSplitter);
            this.textSplitter = textSplitter;
        }

        public List<SingleProcessingData> Errors { get; } = new List<SingleProcessingData>();

        public async Task Tokenize(SingleProcessingData[] documents, int total = 0, bool init = true)
        {
            log.Info("Tokenize documents...");
            List<Task> tasks = documents
                .Select(document => Task.Run(async () => { await ProcessSingle(document, total, init).ConfigureAwait(false); }))
                .ToList();
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public Task Tokenize(SingleProcessingData document, bool init = true)
        {
            log.Info("Tokenize documents...");
            return ProcessSingle(document, 1, init);
        }

        private async Task ProcessSingle(SingleProcessingData document, int total, bool init)
        {
            try
            {
                var result = await textSplitter.Process(new ParseRequest(document.Document) { Date = document.Date }).ConfigureAwait(false);
                if (init)
                {
                    document = new SingleProcessingData(result);
                }

                Interlocked.Increment(ref current);
                log.Info("Tokenize documents {0}/{1}...", current, total);
            }
            catch (Exception ex)
            {
                Errors.Add(document);
                log.Error(ex);
            }
        }
    }
}
