using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Review;

namespace Wikiled.Sentiment.Analysis.Tests.Reviews
{
    [TestFixture]
    public class ConvertNormalizedSentimentTests
    {
        [Test]
        public void Test()
        {
            Assert.AreEqual(SentimentStrength.Weak, 0.06.GetStrength());
            Assert.AreEqual(SentimentStrength.Strong, 0.6.GetStrength());
            Assert.AreEqual(SentimentStrength.Strong, (8.0).GetStrength());
        }
    }
}
