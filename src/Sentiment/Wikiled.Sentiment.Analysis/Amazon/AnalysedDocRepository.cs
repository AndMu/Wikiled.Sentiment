using System.Collections.Generic;
using NLog;
using StackExchange.Redis;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Redis.Keys;
using Wikiled.Redis.Logic;
using Wikiled.Redis.Persistency;
using Wikiled.Redis.Serialization;

namespace Wikiled.Sentiment.Analysis.Amazon
{
    public class AnalysedDocRepository : IRepository
    {
        private readonly IRedisLink manager;

        public AnalysedDocRepository(IRedisLink manager)
        {
            Guard.NotNull(() => manager, manager);
            this.manager = manager;
            manager.RegisterHashType(new DictionarySerializer(new[] {"No"}));
        }

        public string Name => "Analysed";

        public void Save(AmazonReview amazon, Dictionary<string, RedisValue> review)
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            foreach (var redisValue in review)
            {
                value[redisValue.Key] = redisValue.Value.ToString();
            }

            var dataKey = new RepositoryKey(this, new ObjectKey(amazon.Id));
            dataKey.AddIndex(new IndexKey(this, "All", false));
            manager.Client.AddRecord(dataKey, value);
        }
    }
}
