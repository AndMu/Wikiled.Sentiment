using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Wikiled.Common.Utilities.Modules;
using Wikiled.Redis.Config;
using Wikiled.Redis.Modules;

namespace Wikiled.Sentiment.TestLogic.Shared.Helpers
{
    public class RedisServerModule : IModule
    {
        private readonly RedisConfiguration config;

        public RedisServerModule(RedisConfiguration config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public IServiceCollection ConfigureServices(IServiceCollection service)
        {
            service.AddLogging(builder => builder.AddDebug());
            service.AddLogging(builder => builder.AddConsole());
            service.RegisterModule(new RedisModule(new NullLogger<RedisModule>(), config));
            service.RegisterModule<CommonModule>();
            return service;
        }
    }
}
