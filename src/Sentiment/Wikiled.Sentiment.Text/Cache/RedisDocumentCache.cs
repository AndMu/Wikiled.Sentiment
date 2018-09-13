using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NLog;
using Wikiled.Common.Utilities.Helpers;
using Wikiled.Redis.Data;
using Wikiled.Redis.Keys;
using Wikiled.Redis.Logic;
using Wikiled.Redis.Persistency;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Cache
{
    public class RedisDocumentCache : ICachedDocumentsSource, IRepository
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IRedisLink manager;

        private readonly POSTaggerType tagger;

        private readonly ConcurrentDictionary<string, Document> nearCache = new ConcurrentDictionary<string, Document>();

        public RedisDocumentCache(POSTaggerType tagger, IRedisLink manager)
        {
            this.tagger = tagger;
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
            if (!manager.HasDefinition<Document>())
            {
                manager.RegisterNormalized<Document>(new XmlDataSerializer()).IsSingleInstance = true;
            }
        }

        public string Name => $"Cache:{tagger}";

        public async Task<Document> GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(id));
            }

            if (nearCache.TryGetValue(id, out Document doc))
            {
                return doc.CloneJson();
            }

            var key = new RepositoryKey(this, new ObjectKey(id));
            try
            {
                doc = await manager.Client.GetRecords<Document>(key).LastOrDefaultAsync();
                nearCache[id] = doc;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }

            return doc.CloneJson();
        }

        public Task<Document> GetCached(Document original)
        {
            if (original is null)
            {
                throw new ArgumentNullException(nameof(original));
            }

            if (string.IsNullOrEmpty(original.Id))
            {
                throw new ArgumentException("Value cannot be null or empty id.", nameof(original.Id));
            }

            return GetById(original.Id);
        }

        public Task<Document> GetCached(string text)
        {
            return Task.FromResult((Document)null);
        }

        public async Task<bool> Save(Document document)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (string.IsNullOrEmpty(document.Id))
            {
                throw new ArgumentException("Value cannot be null or empty id.", nameof(document.Id));
            }

            nearCache[document.Id] = document;
            var key = new RepositoryKey(this, new ObjectKey(document.Id));
            key.AddIndex(new IndexKey(this, "Index:All", false));
            await manager.Client.AddRecord(key, document).ConfigureAwait(false);
            return true;
        }
    }
}
