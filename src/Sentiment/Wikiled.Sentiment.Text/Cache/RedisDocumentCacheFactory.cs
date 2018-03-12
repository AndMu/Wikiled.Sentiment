using Wikiled.Common.Arguments;
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
            Guard.NotNull(() => redis, redis);
            this.redis = redis;
        }

        public ICachedDocumentsSource Create(POSTaggerType tagger)
        {
            return new RedisDocumentCache(tagger, redis);
        }
    }
}
