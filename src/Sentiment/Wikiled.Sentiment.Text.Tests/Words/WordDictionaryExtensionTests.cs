using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.Text.Helpers;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Tests.Words
{
    [TestFixture]
    public class WordDictionaryExtensionTests
    {
        private Dictionary<string, int> table;

        private MaskDictionary<int> masked;

        [SetUp]
        public void Setup()
        {
            table = new Dictionary<string, int>();
            table["Test"] = 2;
            masked = new MaskDictionary<int>();
            masked["Test"] = 2;

        }
    
        [TestCase("Test", "Test2", true)]
        [TestCase("Test1", "Test", true)]
        [TestCase("Test1", "Test1", false)]
        public void TryGetValue(string word, string raw, bool expected)
        {
            var wordItem = new Mock<IWordItem>();
            wordItem.Setup(item => item.Text).Returns(word);
            wordItem.Setup(item => item.Stemmed).Returns(raw);
            var session = new Mock<ISessionContext>();
            wordItem.Setup(item => item.Session).Returns(session.Object);
            session.Setup(item => item.NGram).Returns(1);
            var relationships = new WordItemRelationships(new Mock<IContextWordsHandler>().Object, wordItem.Object);
            wordItem.Setup(item => item.Relationship).Returns(relationships);

            int result;
            Assert.Throws<ArgumentNullException>(() => table.TryGetValue(null, out result));
            var resultItem = table.TryGetWordValue(wordItem.Object, out result);
            Assert.AreEqual(expected, resultItem);
        }

        [TestCase("Test", "Test2", true)]
        [TestCase("Test1", "Test2", true)]
        [TestCase("Test1", "Test", true)]
        [TestCase("TestAS", "Test1", true)]
        [TestCase("xxxx", "xxx", false)]
        public void MaskedTryGetValue(string word, string raw, bool expected)
        {
            var wordItem = new Mock<IWordItem>();
            wordItem.Setup(item => item.Text).Returns(word);
            wordItem.Setup(item => item.Stemmed).Returns(raw);

            var resultItem = masked.TryGetWordValue(wordItem.Object, out var result);
            Assert.AreEqual(expected, resultItem);
        }
    }
}
