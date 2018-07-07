using System;
using Wikiled.Redis.Logic;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Text.Cache
{
    public class RedisDocumentCacheFactory : ICacheFactory
    {
        private readonly IRedisLink redis;

        public RedisDocumentCacheFactory(IRedisLink redis)
        {
            this.redis = redis ?? throw new ArgumentNullException(nameof(redis));
        }

        public ICachedDocumentsSource Create(POSTaggerType tagger)
        {
            return new RedisDocumentCache(tagger, redis);
        }
    }
}
