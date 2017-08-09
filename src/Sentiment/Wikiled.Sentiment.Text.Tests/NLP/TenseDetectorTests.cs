using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Text.Analysis.POS;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Tests.NLP
{
    [TestFixture]
    public class TenseDetectorTests
    {
        private static readonly object[] testData =
        {
            new object[]
            {
                new[] {"NN", "NN", "NN"},
                TenseType.Unknown
            },

            new object[]
            {
                new[] {"VBD", "NN", "NN"},
                TenseType.Past
            },

            new object[]
            {
                new[] {"VBP", "NN", "NN"},
                TenseType.Present
            },

            new object[]
            {
                new[] {"MD", "NN", "NN"},
                TenseType.Future
            },

            new object[]
            {
                new[] {"MD", "VBP", "NN"},
                TenseType.Future
            },

            new object[]
            {
                new[] {"MD", "VBD", "NN"},
                TenseType.Present
            },

            new object[]
            {
                new[] {"MD", "VBD", "VBD"},
                TenseType.Past
            }
        };

        [TestCaseSource(nameof(testData))]
        public void ResolveTenseInSentence(string[] wordsTags, TenseType expected)
        {
            Assert.Throws<ArgumentNullException>(() => TenseDetector.ResolveTense(null));
            Mock<ISentence> sentence = new Mock<ISentence>();
            List<IWordItem> words = new List<IWordItem>();
            foreach (var type in wordsTags)
            {
                Mock<IWordItem> word = new Mock<IWordItem>();
                word.Setup(item => item.POS).Returns(POSTags.Instance.FindType(type));
                word.Setup(item => item.Text).Returns("will");
                words.Add(word.Object);
            }

            sentence.Setup(item => item.Occurrences).Returns(words);
            var result = sentence.Object.ResolveTense();
            Assert.AreEqual(expected, result);
        }

        [TestCase("VBP", TenseType.Present)]
        [TestCase("VBZ", TenseType.Present)]
        [TestCase("VBD", TenseType.Past)]
        [TestCase("VBN", TenseType.Past)]
        [TestCase("NN", null)]
        public void GetTense(string type, TenseType? tense)
        {
            Mock<IWordItem> word = new Mock<IWordItem>();
            word.Setup(item => item.POS).Returns(POSTags.Instance.FindType(type));
            Assert.Throws<ArgumentNullException>(() => TenseDetector.GetTense(null));
            var result = word.Object.GetTense();
            Assert.AreEqual(tense, result);
        }

        [TestCase("will", TenseType.Future)]
        [TestCase("shall", TenseType.Future)]
        [TestCase("I'll", TenseType.Future)]
        [TestCase("could", TenseType.Past)]
        [TestCase("Would", TenseType.Past)]
        [TestCase("xxx", TenseType.Present)]
        public void GetMDTense(string text, TenseType? tense)
        {
            Mock<IWordItem> word = new Mock<IWordItem>();
            word.Setup(item => item.POS).Returns(POSTags.Instance.MD);
            word.Setup(item => item.Text).Returns(text);
            var result = word.Object.GetTense();
            Assert.AreEqual(tense, result);
        }
    }
}
