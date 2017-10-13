using System;
using System.IO;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Analysis.Tests.Processing
{
    [TestFixture]
    public class Word2VecSentimentAdjusterTests
    {
        private Mock<ISentimentDataHolder> dataHolder;

        private WeightSentimentAdjuster instance;

        [SetUp]
        public void Setup()
        {
            dataHolder = new Mock<ISentimentDataHolder>();
            instance = new WeightSentimentAdjuster(dataHolder.Object);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new WeightSentimentAdjuster(null));
            Assert.IsNotNull(instance);
        }

        [Test]
        public void Adjust()
        {
            Assert.Throws<ArgumentNullException>(() => instance.Adjust(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => instance.Adjust("Test"));
            instance.Adjust(
                Path.Combine(TestContext.CurrentContext.TestDirectory, "Processing", "Electronics.csv"));
            dataHolder.Verify(item => item.Adjust(It.IsAny<string>(), It.IsAny<double>()), Times.Exactly(9));
        }

        [Test]
        public void AdjustWeight()
        {
            SentimentDataHolder holder = new SentimentDataHolder();
            holder.AddValue("affection", new SentimentValueData(10));
            var sentiment = holder.MeasureSentiment(new TestWordItem {Text = "affection"}).DataValue.Value;
            Assert.AreEqual(10, sentiment);
            instance = new WeightSentimentAdjuster(holder);
            instance.Adjust(Path.Combine(TestContext.CurrentContext.TestDirectory, "Processing", "Electronics.csv"));
            sentiment = holder.MeasureSentiment(new TestWordItem {Text = "affection"}).DataValue.Value;
            Assert.AreEqual(4.5, sentiment);
        }
    }
}