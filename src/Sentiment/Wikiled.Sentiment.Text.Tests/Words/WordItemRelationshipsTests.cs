using System;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Tests.Words
{
    [TestFixture]
    public class WordItemRelationshipsTests
    {
        private Mock<IContextWordsHandler> handler;

        private TestWordItem parent;

        private WordItemRelationships instance;

        [SetUp]
        public void Setup()
        {
            handler = new Mock<IContextWordsHandler>();
            handler.Setup(item => item.Context).Returns(new SessionContext(new NullLogger<SessionContext>()));
            parent = new TestWordItem("Text");
            parent.WordIndex = 1;
            instance = new WordItemRelationships(handler.Object, parent);
            parent.Relationship = instance;
        }

        [Test]
        public void Related()
        {
            Assert.AreEqual(0, instance.Related.Count());
            var word = new Mock<IWordItem>();
            word.Setup(item => item.WordIndex).Returns(0);
            instance.Add(word.Object);
            Assert.AreEqual(1, instance.Related.Count());
            word = new Mock<IWordItem>();
            word.Setup(item => item.WordIndex).Returns(2);
            instance.Add(word.Object);
            Assert.AreEqual(2, instance.Related.Count());
        }

        [Test]
        public void Create()
        {
            Assert.Throws<ArgumentNullException>(() => new WordItemRelationships(handler.Object, null));
            Assert.AreEqual(parent, instance.Owner);
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
            var previousRelationship = new WordItemRelationships(handler.Object, previous.Object);
            previousRelationship.Next = instance.Owner;
            previous.Setup(item => item.Relationship).Returns(previousRelationship);
            previous.Setup(item => item.IsInvertor).Returns(true);
            instance.Previous = previous.Object;
            Assert.AreEqual(previous.Object, instance.Inverted);
        }


        [Test]
        public void Views()
        {
            var wordItem = ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("running", "NN");
            var result = wordItem.Relationship.Views;
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("running", result[0]);
            Assert.AreEqual("run", result[1]);
        }

        [Test]
        public void PhraseViews()
        {
            var wordItem = ActualWordsHandler.InstanceSimple.WordFactory.CreatePhrase("NNS");
            wordItem.Add(ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("so", "JJ"));
            wordItem.Add(ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("good", "JJ"));
            var result = wordItem.Relationship.Views;
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("so good", result[0]);
        }

        [Test]
        public void NGrams()
        {
            ActualWordsHandler.InstanceSimple.Container.Context.NGram = 3;
            var wordItem = ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("running", "NN");
            var second = ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("so", "JJ");
            second.WordIndex = 1;
            wordItem.Relationship.Add(second);
            var third = ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("good", "JJ");
            third.WordIndex = 2;
            wordItem.Relationship.Add(third);
            var result = wordItem.Relationship.Views;
            
            Assert.AreEqual(4, result.Length);
            Assert.AreEqual("running so good", result[0]);
            Assert.AreEqual("running so", result[1]);
            Assert.AreEqual("running", result[2]);
            Assert.AreEqual("run", result[3]);
        }

        [Test]
        public void NGrams2()
        {
            ActualWordsHandler.InstanceSimple.Container.Context.NGram = 3;
            var wordItem = ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("running", "NN");
            var second = ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("so", "JJ");
            second.WordIndex = 1;
            second.Relationship.Add(wordItem);
            var third = ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("good", "JJ");
            third.WordIndex = 2;
            second.Relationship.Add(third);
            var result = second.Relationship.Views;
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("so good", result[0]);
            Assert.AreEqual("so", result[1]);
        }
    }
}
