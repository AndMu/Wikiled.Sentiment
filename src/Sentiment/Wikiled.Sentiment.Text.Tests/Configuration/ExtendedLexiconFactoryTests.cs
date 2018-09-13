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
            Mock<IConfigurationHandler> configuration = new Mock<IConfigurationHandler>();
            configuration.Setup(item => item.ResolvePath("Resources")).Returns(@"c:/data");
            configuration.Setup(item => item.SafeGetConfiguration("Lexicon", @"Library/Standard")).Returns(@"c:/data");
            
            IExtendedLexiconContainer container = new FullLexiconContainerFactory(configuration.Object);
            Assert.AreEqual(@"c:/data", container.ResourcesPath);
            Assert.IsFalse(container.IsConstructed);
        }
    }
}
