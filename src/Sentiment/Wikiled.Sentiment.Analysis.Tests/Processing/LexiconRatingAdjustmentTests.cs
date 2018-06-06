using System;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Analysis.Tests.Processing
{
    [TestFixture]
    public class LexiconRatingAdjustmentTests
    {
        private Mock<IParsedReview> mockParsedReview;

        private SentimentDataHolder sentimentData;

        private LexiconRatingAdjustment instance;

        [SetUp]
        public void SetUp()
        {
            mockParsedReview = new Mock<IParsedReview>();
            sentimentData = new SentimentDataHolder();
            sentimentData.SetValue("good", new SentimentValueData(2, SentimentSource.Word2Vec));
            instance = CreateInstance();
            mockParsedReview.Setup(item => item.Items).Returns(
                new IWordItem[]
                {
                    new TestWordItem
                    {
                        Text = "Bad",
                        Stemmed = "Bad"
                    },
                    new TestWordItem
                    {
                        Text = "Good",
                        Stemmed = "Good"
                    }
                });
        }

        [Test]
        public void CalculateRating()
        {
            instance.CalculateRating();
            Assert.AreEqual(5, instance.Rating.StarsRating);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new LexiconRatingAdjustment(null, sentimentData));
            Assert.Throws<ArgumentNullException>(() => new LexiconRatingAdjustment(mockParsedReview.Object, null));
        }

        private LexiconRatingAdjustment CreateInstance()
        {
            return new LexiconRatingAdjustment(
                mockParsedReview.Object,
                sentimentData);
        }
    }
}
