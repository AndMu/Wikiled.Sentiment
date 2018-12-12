using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.Resources;

namespace Wikiled.Sentiment.Text.Tests.Configuration
{
    [TestFixture]
    public class ExtendedLexiconFactoryTests
    {
        [Test]
        public void ConstructUsingConfiguration()
        {
            var configuration = new Mock<IConfigurationHandler>();
            configuration.Setup(item => item.ResolvePath("Resources")).Returns(@"c:/data");
            configuration.Setup(item => item.SafeGetConfiguration("Lexicon", @"Library/Standard")).Returns(TestContext.CurrentContext.TestDirectory);
            var container = new LexiconConfiguration(configuration.Object);
        }
    }
}
