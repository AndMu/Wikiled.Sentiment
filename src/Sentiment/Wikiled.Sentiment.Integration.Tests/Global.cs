using System.Configuration;
using System.IO;
using NUnit.Framework;
using Wikiled.Redis.Logic;

namespace Wikiled.Sentiment.Integration.Tests
{
    [SetUpFixture]
    public class Global
    {
        private RedisProcessManager manager;

        [OneTimeSetUp]
        public void Setup()
        {
            manager = new RedisProcessManager();
            var redis = Path.Combine(TestContext.CurrentContext.TestDirectory, ConfigurationManager.AppSettings["redis"]);
            manager.Start(redis);
        }

        [OneTimeTearDown]
        public void Clean()
        {
            manager.Dispose();
        }
    }
}
