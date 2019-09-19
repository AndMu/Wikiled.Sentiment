using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Tests.Aspects
{
    [TestFixture]
    public class AspectSerializerTests
    {
        private AspectDectector instance;

        private AspectSerializer serializer;

        [SetUp]
        public void Setup()
        {
            var words = new List<IWordItem>();
            for (var i = 0; i < 100; i++)
            {
                var word = new Mock<IWordItem>();
                word.Setup(item => item.Text).Returns(i.ToString);
                words.Add(word.Object);
            }

            instance = new AspectDectector(words.ToArray(), words.ToArray());
            serializer = (AspectSerializer)ActualWordsHandler.InstanceSimple.Container.Resolve<IAspectSerializer>();
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new AspectSerializer(null));
        }

        [Test]
        public void Serialize()
        {
            System.Xml.Linq.XDocument document = serializer.Serialize(instance);
            IAspectDectector result = serializer.Deserialize(document);
            Assert.AreEqual(100, result.AllAttributes.Count());
            Assert.AreEqual(100, result.AllFeatures.Count());
        }
    }
}
