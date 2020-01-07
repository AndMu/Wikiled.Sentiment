using System;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Utilities.Serialization;
using Wikiled.Redis.Logic;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Text.Cache
{
    public class RedisDocumentCacheFactory : ICacheFactory
    {
        private readonly IRedisLink redis;

        private readonly LocalDocumentsCache local;

        private readonly ILoggerFactory factory;

        private IJsonSerializer serializer;

        public RedisDocumentCacheFactory(ILoggerFactory factory, IRedisLink redis, LocalDocumentsCache local, IJsonSerializer serializer)
        {
            this.redis = redis ?? throw new ArgumentNullException(nameof(redis));
            this.local = local ?? throw new ArgumentNullException(nameof(local));
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public ICachedDocumentsSource Create(POSTaggerType tagger)
        {
            return new RedisDocumentCache(factory.CreateLogger<RedisDocumentCache>(), tagger, redis, local, serializer);
        }
    }
}
