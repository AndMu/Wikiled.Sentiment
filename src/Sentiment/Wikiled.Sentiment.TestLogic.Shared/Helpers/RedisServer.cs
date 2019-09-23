using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Wikiled.Common.Utilities.Modules;
using Wikiled.Redis.Config;
using Wikiled.Redis.Modules;

namespace Wikiled.Sentiment.TestLogic.Shared.Helpers
{
    public class RedisServer
    {
        public RedisServer(RedisConfiguration config)
        {
            var service = new ServiceCollection();
            service.AddLogging(builder => builder.AddDebug());
            service.AddLogging(builder => builder.AddConsole());
            service.RegisterModule(new RedisModule(new NullLogger<RedisModule>(), config));
            service.RegisterModule<CommonModule>();
            Provider = service.BuildServiceProvider();
        }

        public ServiceProvider Provider { get; }
    }
}
