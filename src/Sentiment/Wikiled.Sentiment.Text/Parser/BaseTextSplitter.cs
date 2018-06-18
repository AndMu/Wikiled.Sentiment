using System.Threading.Tasks;
using NLog;
using Wikiled.Common.Arguments;
using Wikiled.Common.Extensions;
using Wikiled.Common.Logging;
using Wikiled.Common.Serialization;
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
            using (new PerformanceTrace(log.Debug, "Process"))
            {
                request.Document.Text = handler.Repair.Repair(request.Document.Text);
                if (string.IsNullOrWhiteSpace(request.Document.Id))
                {
                    var tag = request.Document.Id = GenerateKey(request.Document.Text);
                    log.Debug("Key not found on document. generating: {0}...", tag);
                }

                Document document = await cache.GetCached(request.Document).ConfigureAwait(false);
                if (document == null)
                {
                    string text = request.Document.Text.Trim();
                    if (string.IsNullOrEmpty(text))
                    {
                        document = new Document();
                    }
                    else
                    {
                        text = text.SanitizeXmlString();
                        document = await cache.GetCached(text).ConfigureAwait(false);
                    }
                }

                if (document != null)
                {
                    log.Debug("Cache HIT");
                    document.Id = request.Document.Id;
                    return document;
                }

                document = await Task.Run(() => ActualProcess(request)).ConfigureAwait(false);
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

        private string GenerateKey(string text)
        {
            int total = text.Length < 10 ? text.Length : 10;
            var beggining = text.Substring(0, total).CreatePureLetterText();
            var ending = text.Substring(text.Length - total, total).CreatePureLetterText();
            var length = text.Length;
            return string.Format(
                "{0}{3}{1}{4}{2}",
                beggining,
                ending,
                length,
                "__End__",
                "__Len__");
        }
    }
}