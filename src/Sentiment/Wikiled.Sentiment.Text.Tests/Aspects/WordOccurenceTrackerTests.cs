using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Tests.Aspects
{
    [TestFixture]
    public class WordOccurenceTrackerTests
    {
        private WordOccurenceTracker instance;

        [SetUp]
        public void Setup()
        {
            instance = new WordOccurenceTracker();
        }

        [Test]
        public void Construct()
        {
            Assert.AreEqual(0, instance.Total);
            Assert.AreEqual(0, instance.Words.ToArray().Length);
        }

        [Test]
        public void AddWord()
        {
            Assert.Throws<ArgumentNullException>(() => instance.AddWord(null));
            var word = new Mock<IWordItem>();
            word.Setup(item => item.Text).Returns("Word");
            instance.AddWord(word.Object);
            Assert.AreEqual(1, instance.Total);
            Assert.AreEqual(1, instance.Words.ToArray().Length);

            instance.AddWord(word.Object);
            Assert.AreEqual(2, instance.Total);
            Assert.AreEqual(1, instance.Words.ToArray().Length);

            word.Setup(item => item.Text).Returns("Word2");
            instance.AddWord(word.Object);
            Assert.AreEqual(3, instance.Total);
            Assert.AreEqual(2, instance.Words.ToArray().Length);
        }

        [Test]
        public void AddPhrase()
        {
            Assert.Throws<ArgumentNullException>(() => instance.AddPhrase(null));
            var word = new Mock<IPhrase>();
            word.Setup(item => item.Text).Returns("Word");
            instance.AddPhrase(word.Object);
            Assert.AreEqual(0, instance.Total);
            Assert.AreEqual(1, instance.GetPhrases(0).ToArray().Length);

            instance.AddPhrase(word.Object);
            Assert.AreEqual(0, instance.Total);
            Assert.AreEqual(1, instance.GetPhrases(0).ToArray().Length);

            word.Setup(item => item.Text).Returns("Word2");
            instance.AddPhrase(word.Object);
            Assert.AreEqual(0, instance.Total);
            Assert.AreEqual(2, instance.GetPhrases(0).ToArray().Length);
            Assert.AreEqual(0, instance.Words.ToArray().Length);
        }
    }
}
