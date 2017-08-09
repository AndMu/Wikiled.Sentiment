using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Tests.Words
{
    [TestFixture]
    public class WordItemRelationshipsTests
    {
        private Mock<IWordsHandler> handler;

        private Mock<IWordItem> parent;

        private WordItemRelationships instance;

        [SetUp]
        public void Setup()
        {
            handler = new Mock<IWordsHandler>();
            parent = new Mock<IWordItem>();
            parent.Setup(item => item.WordIndex).Returns(1);
            instance = new WordItemRelationships(handler.Object, parent.Object);
        }

        [Test]
        public void Related()
        {
            Assert.AreEqual(0, instance.PriorRelated.Count());
            Assert.AreEqual(0, instance.AfterRelated.Count());
            var word = new Mock<IWordItem>();
            word.Setup(item => item.WordIndex).Returns(0);
            instance.Add(word.Object);
            Assert.AreEqual(1, instance.PriorRelated.Count());
            Assert.AreEqual(0, instance.AfterRelated.Count());
            word = new Mock<IWordItem>();
            word.Setup(item => item.WordIndex).Returns(2);
            instance.Add(word.Object);
            Assert.AreEqual(1, instance.PriorRelated.Count());
            Assert.AreEqual(1, instance.AfterRelated.Count());
        }

        [Test]
        public void Create()
        {
            Assert.Throws<ArgumentNullException>(() => new WordItemRelationships(handler.Object, null));
            Assert.AreEqual(parent.Object, instance.Owner);
            Assert.IsNull(instance.Previous);
            Assert.IsNull(instance.Next);
            Assert.IsNull(instance.Sentiment);
            Assert.IsNull(instance.Inverted);
            Assert.IsNull(instance.Part);
        }

        [Test]
        public void SimpleInverted()
        {
            var previous = new Mock<IWordItem>();
            previous.Setup(item => item.IsInvertor).Returns(true);
            instance.Previous = previous.Object;
            Assert.AreEqual(previous.Object, instance.Inverted);
        }
    }
}
