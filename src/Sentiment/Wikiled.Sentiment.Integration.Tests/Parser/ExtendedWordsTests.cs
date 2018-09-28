using NUnit.Framework;
using System.Linq;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Integration.Tests.Parser
{
    [TestFixture]
    public class ExtendedWordsTests
    {
        [Test]
        public void GetSentiments()
        {
            var result = ActualWordsHandler.InstanceOpen.Container.Resolve<IExtendedWords>().GetSentiments().ToArray();
            Assert.AreEqual(176, result.Length);
        }

        [Test]
        public void GetReplacements()
        {
            var result = ActualWordsHandler.InstanceOpen.Container.Resolve<IExtendedWords>().GetReplacements().ToArray();
            Assert.AreEqual(186, result.Length);
        }
    }
}
