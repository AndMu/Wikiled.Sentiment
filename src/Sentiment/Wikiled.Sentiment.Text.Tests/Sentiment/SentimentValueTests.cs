using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Tests.Sentiment
{
    [TestFixture]
    public class SentimentValueTests
    {
        private Mock<IWordItem> wordItem;

        [SetUp]
        public void Setup()
        {
            wordItem = new Mock<IWordItem>();
        }

        [Test]
        public void Netgative()
        {
            SentimentValue value = new SentimentValue(wordItem.Object, - 0.5);
            Assert.AreEqual(-0.5, value.DataValue.Value);
            Assert.IsFalse(value.DataValue.IsPositive);
        }

        [Test]
        public void Positive()
        {
            SentimentValue value = new SentimentValue(wordItem.Object, 0.5);
            Assert.AreEqual(0.5, value.DataValue.Value);
            Assert.IsTrue(value.DataValue.IsPositive);
        }

        [Test]
        public void CreateGood()
        {
            var value = SentimentValue.CreateGood(wordItem.Object);
            Assert.AreEqual(1, value.DataValue.Value);
            Assert.IsTrue(value.DataValue.IsPositive);
        }

        [Test]
        public void CreateBad()
        {
            var value = SentimentValue.CreateBad(wordItem.Object);
            Assert.AreEqual(-1, value.DataValue.Value);
            Assert.IsFalse(value.DataValue.IsPositive);
        }

        [Test]
        public void GetDistanced()
        {
            var value = SentimentValue.CreateBad(wordItem.Object);
            var distanced = value.GetDistanced(1);
            Assert.AreEqual(-1, distanced.DataValue.Value);
            distanced = value.GetDistanced(2);
            Assert.AreEqual(-1, distanced.DataValue.Value);
            distanced = value.GetDistanced(3);
            Assert.AreEqual(-0.5, distanced.DataValue.Value);
            distanced = value.GetDistanced(5);
            Assert.AreEqual(-0.25, distanced.DataValue.Value);
        }
    }
}
