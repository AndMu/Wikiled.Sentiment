using System;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Analysis.Tests.Processing
{
    [TestFixture]
    public class LexiconRatingAdjustmentTests
    {
        private Mock<IParsedReview> mockParsedReview;

        private Mock<ISentimentDataReader> mockSentimentDataReader;

        private LexiconRatingAdjustment instance;

        [SetUp]
        public void SetUp()
        {
            mockParsedReview = new Mock<IParsedReview>();
            mockSentimentDataReader = new Mock<ISentimentDataReader>();
            instance = CreateInstance();
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new LexiconRatingAdjustment(null, mockSentimentDataReader.Object));
            Assert.Throws<ArgumentNullException>(() => new LexiconRatingAdjustment(mockParsedReview.Object, null));
        }

        private LexiconRatingAdjustment CreateInstance()
        {
            return new LexiconRatingAdjustment(
                mockParsedReview.Object,
                mockSentimentDataReader.Object);
        }
    }
}
