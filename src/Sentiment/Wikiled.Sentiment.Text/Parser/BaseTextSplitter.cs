using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Wikiled.Common.Logging;
using Wikiled.Common.Utilities.Helpers;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Parser
{
    public abstract class BaseTextSplitter : ITextSplitter
    {
        private readonly ILogger log;

        private readonly ICachedDocumentsSource cache;

        protected BaseTextSplitter(ILogger log, ICachedDocumentsSource cache)
        {
            this.cache = cache ?? throw new System.ArgumentNullException(nameof(cache));
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public virtual void Dispose()
        {
        }

        public async Task<Document> Process(ParseRequest request)
        {
            if (request?.Document == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            using (new PerformanceTrace(item => log.LogDebug(item), "Process"))
            {
                var text = request.Document.Text.Trim();
                request.Document.Text = text;
                if (string.IsNullOrWhiteSpace(request.Document.Id))
                {
                    var tag = request.Document.Id = Guid.NewGuid().ToString();
                    log.LogDebug("Key not found on document. generating: {0}...", tag);
                }

                Document document = await cache.GetCached(request.Document).ConfigureAwait(false);
                if (document != null)
                {
                    log.LogDebug("Cache HIT");
                    document = document.CloneJson();
                    document.Id = request.Document.Id;
                    return document;
                }

                if (!string.IsNullOrEmpty(text))
                {
                    document = await Task.Run(() => ActualProcess(request)).ConfigureAwait(false);
                }
                else
                {
                    document = new Document();
                    log.LogInformation("Empty document detected");
                }

                document.Id = request.Document.Id;
                document.DocumentTime = request.Document.DocumentTime;
                document.Author = request.Document.Author;
                document.Title = request.Document.Title;
                if (await cache.Save(document).ConfigureAwait(false))
                {
                    return document;
                }

                return document;
            }
        }

        protected abstract Document ActualProcess(ParseRequest request);


    }
}