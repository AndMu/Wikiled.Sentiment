using NLog;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Wikiled.Common.Utilities.Helpers;
using Wikiled.Redis.Data;
using Wikiled.Redis.Keys;
using Wikiled.Redis.Logic;
using Wikiled.Redis.Persistency;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.Extensions;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Cache
{
    public class RedisDocumentCache : ICachedDocumentsSource, IRepository
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IRedisLink manager;

        private readonly POSTaggerType tagger;

        private readonly LocalDocumentsCache local;

        public RedisDocumentCache(POSTaggerType tagger, IRedisLink manager, LocalDocumentsCache local)
        {
            this.tagger = tagger;
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
            this.local = local ?? throw new ArgumentNullException(nameof(local));
            if (!manager.HasDefinition<Document>())
            {
                manager.RegisterNormalized<Document>(new XmlDataSerializer()).IsSingleInstance = true;
            }
        }

        public string Name => $"Cache:{tagger}";

        public async Task<Document> GetCached(Document original)
        {
            if (original is null)
            {
                throw new ArgumentNullException(nameof(original));
            }

            if (string.IsNullOrEmpty(original.Id))
            {
                throw new ArgumentException("Value cannot be null or empty id.", nameof(original.Id));
            }

            Document result = await local.GetCached(original).ConfigureAwait(false);
            if (result != null)
            {
                log.Debug("Found in local cache");
                return result;
            }

            RepositoryKey key = new RepositoryKey(this, new ObjectKey(original.Id));
            result = await manager.Client.GetRecords<Document>(key).LastOrDefaultAsync();
            if (result != null)
            {
                if (result.Text == original.Text)
                {
                    await local.Save(result).ConfigureAwait(false);
                    return result;
                }

                log.Warn("Mistmatch in document text: {0}", original.Id);
            }

            result = await GetById(original.GetId(), original).ConfigureAwait(false);
            if (result != null)
            {
                return result;
            }

            return await GetById(original.GetTextId(), original);
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

            await local.Save(document).ConfigureAwait(false);
            RepositoryKey key = new RepositoryKey(this, new ObjectKey(document.Id));
            key.AddIndex(new IndexKey(this, "Index:All", false));
            key.AddIndex(new IndexKey(this, $"Index:{document.GetId()}", true));
            key.AddIndex(new IndexKey(this, $"Index:{document.GetTextId()}", true));
            await manager.Client.AddRecord(key, document.CloneJson()).ConfigureAwait(false);
            return true;
        }

        private async Task<Document> GetById(string id, Document original)
        {
            IndexKey index = new IndexKey(this, id, true);
            Document result = await manager.Client.GetRecords<Document>(index).LastOrDefaultAsync();
            if (result?.Text == original.Text)
            {
                await local.Save(result).ConfigureAwait(false);
                return result;
            }

            log.Warn("Mistmatch in document text: {0}", id);
            return null;
        }
    }
}
