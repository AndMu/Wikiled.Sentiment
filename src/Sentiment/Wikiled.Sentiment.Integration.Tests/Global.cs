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
            manager.Start(TestContext.CurrentContext.TestDirectory);
        }

        [OneTimeTearDown]
        public void Clean()
        {
            manager.Dispose();
        }
    }
}
