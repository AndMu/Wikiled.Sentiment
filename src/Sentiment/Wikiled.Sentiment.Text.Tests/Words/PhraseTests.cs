using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Text.Analysis.POS;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Tests.Words
{
    [TestFixture]
    public class PhraseTests
    {
        private WordsHandlerHelper helper;

        private Phrase instance;

        [SetUp]
        public void Setup()
        {
            helper = new WordsHandlerHelper();
            helper.RawTextExractor.Setup(item => item.GetWord("test")).Returns("T");
            helper.Handler.Setup(item => item.CheckSentiment(It.IsAny<WordOccurrence>())).Returns(new SentimentValue(new TestWordItem(string.Empty), "Text", 2));
            helper.Handler.Setup(item => item.IsStop(It.IsAny<WordOccurrence>())).Returns(false);
            helper.Handler.Setup(item => item.MeasureQuantifier(It.IsAny<WordOccurrence>())).Returns(2);
            instance = Phrase.Create(helper.Handler.Object, POSTags.Instance.NN);
        }

        [Test]
        public void Create()
        {
            Assert.Throws<ArgumentNullException>(() => Phrase.Create(helper.Handler.Object, null));
            Assert.Throws<ArgumentNullException>(() => Phrase.Create(null, POSTags.Instance.NN));

            Assert.AreEqual(string.Empty, instance.Text);
            Assert.AreEqual(string.Empty, instance.Stemmed);
            Assert.AreEqual("NN", instance.POS.Tag);
            Assert.IsNotNull(instance.Relationship);
            Assert.IsFalse(instance.IsSentiment);
            Assert.IsFalse(instance.IsTopAttribute);
            Assert.IsNull(instance.QuantValue);
            Assert.IsFalse(instance.IsQuestion);
            Assert.IsFalse(instance.IsFeature);
            Assert.IsFalse(instance.IsFixed);
            Assert.IsFalse(instance.IsStopWord);
            Assert.IsFalse(instance.IsSimple);
            Assert.AreEqual(0, instance.AllWords.Count());
        }

        [Test]
        public void Add()
        {
            Assert.Throws<ArgumentNullException>(() => instance.Add(null));
            instance.Add(WordOccurrence.Create(helper.Handler.Object, helper.RawTextExractor.Object, helper.InquirerManager.Object, "Test", null, POSTags.Instance.NN));
            instance.Add(WordOccurrence.Create(helper.Handler.Object, helper.RawTextExractor.Object, helper.InquirerManager.Object, "Test", null, POSTags.Instance.NN));

            Assert.AreEqual("test test", instance.Text);
            Assert.AreEqual("t t", instance.Stemmed);
            Assert.AreEqual("NN", instance.POS.Tag);
            Assert.IsNotNull(instance.Relationship);
            Assert.IsFalse(instance.IsSentiment);
            Assert.IsFalse(instance.IsTopAttribute);
            Assert.AreEqual(2, instance.QuantValue);
            Assert.IsFalse(instance.IsQuestion);
            Assert.IsFalse(instance.IsFeature);
            Assert.IsFalse(instance.IsFixed);
            Assert.IsFalse(instance.IsStopWord);
            Assert.IsFalse(instance.IsSimple);
            Assert.AreEqual(2, instance.AllWords.Count());
        }

        [Test]
        public void CreateFixed()
        {
            instance.Add(WordOccurrence.Create(helper.Handler.Object, helper.RawTextExractor.Object, helper.InquirerManager.Object, "xxxbad", null, POSTags.Instance.NN));
            Assert.IsTrue(instance.IsFixed);
        }
    }
}
