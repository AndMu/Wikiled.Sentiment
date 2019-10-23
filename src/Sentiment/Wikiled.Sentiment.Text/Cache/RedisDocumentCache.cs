using Microsoft.Extensions.Logging;
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
using Wikiled.Text.Analysis.Structure.Light;

namespace Wikiled.Sentiment.Text.Cache
{
    public class RedisDocumentCache : ICachedDocumentsSource, IRepository
    {
        private readonly ILogger<RedisDocumentCache> log;

        private readonly IRedisLink manager;

        private readonly POSTaggerType tagger;

        private readonly LocalDocumentsCache local;

        public RedisDocumentCache(ILogger<RedisDocumentCache> log, POSTaggerType tagger, IRedisLink manager, LocalDocumentsCache local)
        {
            this.tagger = tagger;
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
            this.local = local ?? throw new ArgumentNullException(nameof(local));
            this.log = log ?? throw new ArgumentNullException(nameof(log));

            if (!manager.HasDefinition<Document>())
            {
                manager.RegisterNormalized<Document>(new XmlDataSerializer()).IsSingleInstance = true;
            }
        }

        public string Name => $"Cache:{tagger}";

        public async Task<LightDocument> GetCached(IDocument original)
        {
            if (original is null)
            {
                throw new ArgumentNullException(nameof(original));
            }

            if (string.IsNullOrEmpty(original.Id))
            {
                throw new ArgumentException("Value cannot be null or empty id.", nameof(original.Id));
            }

            var result = await local.GetCached(original).ConfigureAwait(false);
            if (result != null)
            {
                log.LogDebug("Found in local cache");
                return result;
            }

            var key = new RepositoryKey(this, new ObjectKey(original.Id));
            result = await manager.Client.GetRecords<LightDocument>(key).LastOrDefaultAsync();
            if (result != null)
            {
                if (result.Text == original.Text)
                {
                    await local.Save(result).ConfigureAwait(false);
                    return result;
                }

                log.LogWarning("Mistmatch in document text: {0}", original.Id);
            }

            result = await GetById(original.GetId(), original).ConfigureAwait(false);
            if (result != null)
            {
                return result;
            }

            return await GetById(original.GetTextId(), original);
        }

        public async Task<bool> Save(LightDocument document)
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
            var key = new RepositoryKey(this, new ObjectKey(document.Id));
            key.AddIndex(new IndexKey(this, "Index:All", false));
            key.AddIndex(new IndexKey(this, $"Index:{document.GetId()}", true));
            key.AddIndex(new IndexKey(this, $"Index:{document.GetTextId()}", true));
            await manager.Client.AddRecord(key, document.CloneJson()).ConfigureAwait(false);
            return true;
        }

        private async Task<LightDocument> GetById(string id, IDocument original)
        {
            var index = new IndexKey(this, id, true);
            var result = await manager.Client.GetRecords<LightDocument>(index).LastOrDefaultAsync();
            if (result?.Text == original.Text)
            {
                await local.Save(result).ConfigureAwait(false);
                return result;
            }

            log.LogWarning("Mistmatch in document text: {0}", id);
            return null;
        }
    }
}
