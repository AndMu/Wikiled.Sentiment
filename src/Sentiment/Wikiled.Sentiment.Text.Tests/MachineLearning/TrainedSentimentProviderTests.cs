using Moq;
using NUnit.Framework;
using System;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Text.Tests.MachineLearning
{
    [TestFixture]
    public class TrainedSentimentProviderTests
    {
        private Mock<ISentimentProvider> mockSentimentProvider;

        private Mock<ITrainingPerspective> mockTrainingPerspective;

        private TrainedSentimentProvider instance;

        [SetUp]
        public void Setup()
        {
            mockSentimentProvider = new Mock<ISentimentProvider>();
            mockTrainingPerspective = new Mock<ITrainingPerspective>();
            instance = CreateProvider();
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new TrainedSentimentProvider(
                null,
                mockTrainingPerspective.Object));
            Assert.Throws<ArgumentNullException>(() => new TrainedSentimentProvider(
                mockSentimentProvider.Object,
                null));
        }

        private TrainedSentimentProvider CreateProvider()
        {
            return new TrainedSentimentProvider(
                mockSentimentProvider.Object,
                mockTrainingPerspective.Object);
        }
    }
}
