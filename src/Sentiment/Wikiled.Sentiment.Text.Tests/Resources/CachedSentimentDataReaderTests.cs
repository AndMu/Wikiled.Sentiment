using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Text.Tests.Resources
{
    [TestFixture]
    public class CachedSentimentDataReaderTests
    {
        private Mock<ISentimentDataReader> mockSentimentDataReader;

        private CachedSentimentDataReader instance;

        [SetUp]
        public void SetUp()
        {
            mockSentimentDataReader = new Mock<ISentimentDataReader>();
            instance = CreateInstance();
        }
      
        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new CachedSentimentDataReader(null));
        }

        [Test]
        public void Read()
        {
            var result = new List<WordSentimentValueData>();
            mockSentimentDataReader.Setup(item => item.Read()).Returns(result);
            var actual = instance.Read();
            Assert.AreSame(result, actual);
            actual = instance.Read();
            Assert.AreSame(result, actual);
            mockSentimentDataReader.Verify(item => item.Read(), Times.Once);
        }

        private CachedSentimentDataReader CreateInstance()
        {
            return new CachedSentimentDataReader(mockSentimentDataReader.Object);
        }
    }
}
