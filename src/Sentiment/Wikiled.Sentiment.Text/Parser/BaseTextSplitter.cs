using System.Threading.Tasks;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Logging;
using Wikiled.Core.Utility.Serialization;
using Wikiled.Sentiment.Text.Parser.Cache;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Parser
{
    public abstract class BaseTextSplitter : ITextSplitter
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly ICachedDocumentsSource cache;

        private readonly IWordsHandler handler;

        protected BaseTextSplitter(IWordsHandler handler, ICachedDocumentsSource cache)
        {
            Guard.NotNull(() => handler, handler);
            Guard.NotNull(() => cache, cache);
            this.handler = handler;
            this.cache = cache;
        }

        public virtual void Dispose()
        {
        }

        public async Task<Document> Process(ParseRequest request)
        {
            Guard.NotNull(() => request, request);
            Guard.NotNull(() => request.Document, request.Document);
            using (PerformanceTrace.Debug(log, "Process"))
            {
                request.Document.Text = handler.Repair.Repair(request.Document.Text);
                if (string.IsNullOrWhiteSpace(request.Document.Id))
                {
                    DocumentPersistencyItem key = new DocumentPersistencyItem(request.Document);
                    request.Document.Id = key.Tag;
                    log.Debug("Key not found on document. generating: {0}...", key.Tag);
                }

                Document document = await cache.GetCached(request.Document).ConfigureAwait(false);;
                if (document == null)
                {
                    string text = request.Document.Text.Trim().SanitizeXmlString();
                    document = await cache.GetCached(text).ConfigureAwait(false);
                }

                if (document != null)
                {
                    log.Debug("Cache HIT");
                    document.Id = request.Document.Id;
                    return document;
                }

                document = ActualProcess(request);
                document.Id = request.Document.Id;
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