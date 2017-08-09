using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Tests.Aspects
{
    [TestFixture]
    public class AspectDectectorTests
    {
        private AspectDectector instance;

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
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new AspectDectector(null, new IWordItem[] {}));
            Assert.Throws<ArgumentNullException>(() => new AspectDectector(new IWordItem[] {}, null));
            Assert.AreEqual(100, instance.AllFeatures.Count());
        }

        [TestCase("1", true)]
        [TestCase("x", false)]
        public void IsAspect(string word, bool expected)
        {
            var result = instance.IsAspect(new TestWordItem { Text = word });
            Assert.AreEqual(expected, result);
            result = instance.IsAspect(new TestWordItem { Text = "Unknown", Stemmed = word});
            Assert.AreEqual(expected, result);
        }

        [TestCase("1", true)]
        [TestCase("x", false)]
        public void IsAttribute(string word, bool expected)
        {
            var result = instance.IsAttribute(new TestWordItem { Text = word });
            Assert.AreEqual(expected, result);
            result = instance.IsAttribute(new TestWordItem { Text = "Unknown", Stemmed = word});
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void AddRemove()
        {
            var word = new TestWordItem {Text = "xxx"};
            var result = instance.IsAspect(word);
            Assert.IsFalse(result);
            instance.AddFeature(word);
            result = instance.IsAspect(word);
            Assert.IsTrue(result);
            instance.Remove(word);
            result = instance.IsAspect(word);
            Assert.IsFalse(result);
        }
    }
}
