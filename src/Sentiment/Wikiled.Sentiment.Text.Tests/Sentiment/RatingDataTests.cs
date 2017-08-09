using NUnit.Framework;
using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Text.Tests.Sentiment
{
    [TestFixture]
    public class RatingDataTests
    {
        [Test]
        public void IsStrongPositive()
        {
            RatingData data = new RatingData();
            Assert.IsFalse(data.IsStrong);
            data.Positive += 2;
            Assert.IsFalse(data.IsStrong);
            data.Positive += 2;
            Assert.IsTrue(data.IsStrong);

            data.Negative += 2;
            Assert.IsFalse(data.IsStrong);
            data.Positive += 2;
            Assert.IsTrue(data.IsStrong);
        }

        [Test]
        public void IsStrongNegative()
        {
            RatingData data = new RatingData();
            Assert.IsFalse(data.IsStrong);
            data.Negative += 2;
            Assert.IsFalse(data.IsStrong);
            data.Negative += 2;
            Assert.IsTrue(data.IsStrong);

            data.Positive += 2;
            Assert.IsFalse(data.IsStrong);
            data.Negative += 2;
            Assert.IsTrue(data.IsStrong);
        }
    }
}
