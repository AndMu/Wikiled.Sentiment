using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Text.Tests.Aspects
{
    [TestFixture]
    public class AspectSentimentTrackerTests
    {
        private AspectSentimentTracker instance;

        private Mock<IParsedReview> review;

        private Mock<IContextSentimentFactory> factory;

        private Mock<IContextSentiment> sentiment;

        [SetUp]
        public void Setup()
        {
            factory = new Mock<IContextSentimentFactory>();
            instance = new AspectSentimentTracker(factory.Object);
            review = new Mock<IParsedReview>();
            sentiment = new Mock<IContextSentiment>();
            var words = new[]
                        {
                            new TestWordItem(),
                            new TestWordItem
                            {
                                IsFeature = true,
                                Text = "Aspect1",
                                Stemmed = "Aspect1"
                            },
                            new TestWordItem
                            {
                                IsFeature = true, 
                                Text = "Aspect1",
                                Stemmed = "Aspect1"
                            },
                            new TestWordItem
                            {
                                IsFeature = true,
                                Text = "Aspect2",
                                Stemmed = "Aspect2"
                            }
                        };
            review.Setup(item => item.ImportantWords).Returns(words);
            factory.Setup(item => item.Construct(words[1].Relationship)).Returns(sentiment.Object);
            factory.Setup(item => item.Construct(words[2].Relationship)).Returns(sentiment.Object);
            factory.Setup(item => item.Construct(words[3].Relationship)).Returns(sentiment.Object);
            var sentiments = new List<SentimentValue>();
            sentiments.Add(new SentimentValue(words[0], 10));
            sentiment.Setup(item => item.Sentiments).Returns(sentiments);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new AspectSentimentTracker(null));
        }

        [Test]
        public void Process()
        {
            Assert.Throws<ArgumentNullException>(() => instance.Process(null));
            instance.Process(review.Object);
            var results = instance.GetResults();
            Assert.AreEqual(2, results.Records.Length);
            Assert.AreEqual(1, results.TotalReviews);

            Assert.AreEqual("Aspect1", results.Records[0].Text);
            Assert.AreEqual(1, results.Records[0].Sentiment);
            Assert.AreEqual(2, results.Records[0].Times);

            Assert.AreEqual("Aspect2", results.Records[1].Text);
            Assert.AreEqual(1, results.Records[1].Sentiment);
            Assert.AreEqual(1, results.Records[1].Times);
        }
    }
}
