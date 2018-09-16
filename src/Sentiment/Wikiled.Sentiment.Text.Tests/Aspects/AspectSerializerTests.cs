using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Parser;
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
            List<IWordItem> words = new List<IWordItem>();
            for (int i = 0; i < 100; i++)
            {
                Mock<IWordItem> word = new Mock<IWordItem>();
                word.Setup(item => item.Text).Returns(i.ToString);
                words.Add(word.Object);
            }

            instance = new AspectDectector(words.ToArray(), words.ToArray());
            serializer = new AspectSerializer(ActualWordsHandler.InstanceSimple.WordsHandler);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new AspectSerializer(null));
        }

        [Test]
        public void Serialize()
        {
            var document = serializer.Serialize(instance);
            var result = serializer.Deserialize(document);
            Assert.AreEqual(100, result.AllAttributes.Count());
            Assert.AreEqual(100, result.AllFeatures.Count());
        }
    }
}
