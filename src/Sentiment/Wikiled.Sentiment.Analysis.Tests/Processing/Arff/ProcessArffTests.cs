using NUnit.Framework;
using System;
using Moq;
using Wikiled.Arff.Persistence;
using Wikiled.Arff.Persistence.Headers;
using Wikiled.Sentiment.Analysis.Processing.Arff;

namespace Wikiled.Sentiment.Analysis.Tests.Processing.Arff
{
    [TestFixture]
    public class ProcessArffTests
    {
        private Mock<IArffDataSet> mockArffDataSet;

        private ProcessArff instance;

        [SetUp]
        public void Setup()
        {
            mockArffDataSet = new Mock<IArffDataSet>();
            var header = new Mock<IHeadersWordsHandling>();
            mockArffDataSet.Setup(item => item.Header).Returns(header.Object);
            instance = CreateProcessArff();
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new ProcessArff(null));
            Assert.IsNotNull(instance);
        }

        private ProcessArff CreateProcessArff()
        {
            return new ProcessArff(mockArffDataSet.Object);
        }
    }
}
