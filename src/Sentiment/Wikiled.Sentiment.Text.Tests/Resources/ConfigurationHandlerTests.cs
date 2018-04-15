using NUnit.Framework;
using Wikiled.Sentiment.Text.Resources;

namespace Wikiled.Sentiment.Text.Tests.Resources
{
    [TestFixture]
    public class ConfigurationHandlerTests
    {
        [Test]
        public void ResolvePathRelative()
        {
            var configuration = new ConfigurationHandler();
            configuration.SetConfiguration("Path", @"..\..\Data");
            configuration.StartingLocation = @"C:\my\starting\location";
            string result = configuration.ResolvePath("Path");
            Assert.AreEqual(@"C:\my\Data", result);
        }

        [Test]
        public void ResolvePath()
        {
            var configuration = new ConfigurationHandler();
            configuration.SetConfiguration("Path", @"c:\Data");
            configuration.StartingLocation = @"C:\my\starting\location";
            string result = configuration.ResolvePath("Path");
            Assert.AreEqual(@"c:\Data", result);
        }
    }
}
