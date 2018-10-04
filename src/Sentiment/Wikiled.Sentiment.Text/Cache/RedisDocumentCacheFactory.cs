using System;
using Wikiled.Redis.Logic;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Text.Cache
{
    public class RedisDocumentCacheFactory : ICacheFactory
    {
        private readonly IRedisLink redis;

        private readonly LocalDocumentsCache local;

        public RedisDocumentCacheFactory(IRedisLink redis, LocalDocumentsCache local)
        {
            this.redis = redis ?? throw new ArgumentNullException(nameof(redis));
            this.local = local ?? throw new ArgumentNullException(nameof(local));
        }

        public ICachedDocumentsSource Create(POSTaggerType tagger)
        {
            return new RedisDocumentCache(tagger, redis, local);
        }
    }
}
