using NLog;
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
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly ICachedDocumentsSource cache;

        protected BaseTextSplitter(ICachedDocumentsSource cache)
        {
            this.cache = cache ?? throw new System.ArgumentNullException(nameof(cache));
        }

        public virtual void Dispose()
        {
        }

        public async Task<Document> Process(ParseRequest request)
        {
            if (request?.Document == null)
            {
                throw new System.ArgumentNullException(nameof(request));
            }

            using (new PerformanceTrace(log.Debug, "Process"))
            {
                string text = request.Document.Text.Trim();
                request.Document.Text = text;
                if (string.IsNullOrWhiteSpace(request.Document.Id))
                {
                    string tag = request.Document.Id = Guid.NewGuid().ToString();
                    log.Debug("Key not found on document. generating: {0}...", tag);
                }

                Document document = await cache.GetCached(request.Document).ConfigureAwait(false);
                if (document != null)
                {
                    log.Debug("Cache HIT");
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
                    log.Info("Empty document detected");
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