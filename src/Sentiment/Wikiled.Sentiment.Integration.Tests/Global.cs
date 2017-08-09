using NUnit.Framework;
using Wikiled.Testing.Library.Processes;

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
            manager.Start();
        }

        [OneTimeTearDown]
        public void Clean()
        {
            manager.Dispose();
        }
    }
}
