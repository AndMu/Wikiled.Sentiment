using NUnit.Framework;
using Rhino.Mocks;
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
            MockRepository mock = new MockRepository();
            IConfigurationHandler configuration = mock.StrictMock<IConfigurationHandler>();
            Expect.Call(configuration.ResolvePath("Resources")).Return(@"c:\data");
            Expect.Call(configuration.SafeGetConfiguration("Lexicon", @"Library\Standard")).Return(@"c:\data");
            mock.ReplayAll();

            IExtendedLexiconFactory factory = new ExtendedLexiconFactory(configuration);
            Assert.AreEqual(@"c:\data", factory.ResourcesPath);
            Assert.IsFalse(factory.IsConstructed);
            mock.VerifyAll();
        }
    }
}
