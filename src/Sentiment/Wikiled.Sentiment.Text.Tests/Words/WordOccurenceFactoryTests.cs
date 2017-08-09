using System;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Text.Analysis.POS;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Tests.Words
{
    [TestFixture]
    public class WordOccurenceFactoryTests
    {
        private WordsHandlerHelper helper;

        private WordOccurenceFactory instance;

        [SetUp]
        public void Setup()
        {
            helper = new WordsHandlerHelper();
            instance = new WordOccurenceFactory(helper.Handler.Object);
        }

        [Test]
        public void Create()
        {
            Assert.Throws<ArgumentNullException>(() => new WordOccurenceFactory(null));
            Assert.IsNotNull(instance);
        }

        [Test]
        public void CreateWord()
        {
            var word = instance.CreateWord("Test", POSTags.Instance.NN);
            Assert.IsNotNull(word);
            word = instance.CreateWord("Test", "NN");
            Assert.IsNotNull(word);
        }

        [Test]
        public void CreatePhrase()
        {
            var word = instance.CreatePhrase("NN");
            Assert.IsNotNull(word);
        }
    }
}
