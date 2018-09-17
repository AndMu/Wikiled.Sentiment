using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Text.Analysis.POS;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Tests.Words
{
    [TestFixture]
    public class WordOccurrenceTests
    {
        private WordsHandlerHelper helper;

        private WordOccurrence instance;

        [SetUp]
        public void Setup()
        {
            helper = new WordsHandlerHelper();
            helper.RawTextExractor.Setup(item => item.GetWord("test")).Returns("T");
            helper.Handler.Setup(item => item.IsSentiment(It.IsAny<WordOccurrence>())).Returns(true);
            helper.Handler.Setup(item => item.IsStop(It.IsAny<WordOccurrence>())).Returns(true);
            helper.Handler.Setup(item => item.MeasureQuantifier(It.IsAny<WordOccurrence>())).Returns(2);
            instance = WordOccurrence.Create(helper.Handler.Object, helper.RawTextExractor.Object, helper.InquirerManager.Object, "Test", null, POSTags.Instance.NN);
        }

        [Test]
        public void Create()
        {
            Assert.Throws<ArgumentNullException>(() => WordOccurrence.Create(null, helper.RawTextExractor.Object, helper.InquirerManager.Object, "Test", null, POSTags.Instance.NN));
            Assert.Throws<ArgumentNullException>(() => WordOccurrence.Create(helper.Handler.Object, null, helper.InquirerManager.Object, "Test", null, POSTags.Instance.NN));
            Assert.Throws<ArgumentNullException>(() => WordOccurrence.Create(helper.Handler.Object, helper.RawTextExractor.Object, null, "Test", null, POSTags.Instance.NN));
            Assert.Throws<ArgumentException>(() => WordOccurrence.Create(helper.Handler.Object, helper.RawTextExractor.Object, helper.InquirerManager.Object, null, null, POSTags.Instance.NN));
            Assert.Throws<ArgumentNullException>(() => WordOccurrence.Create(helper.Handler.Object, helper.RawTextExractor.Object, helper.InquirerManager.Object, "Test", null, null));
            Assert.Throws<ArgumentException>(() => WordOccurrence.Create(helper.Handler.Object, helper.RawTextExractor.Object, helper.InquirerManager.Object, "Test", null, POSTags.Instance.SBAR));

            Assert.AreEqual("test", instance.Text);
            Assert.AreEqual("t", instance.Stemmed);
            Assert.AreEqual("NN", instance.POS.Tag);
            Assert.AreEqual(NamedEntities.None, instance.Entity);
            Assert.IsNotNull(instance.Relationship);
            Assert.IsTrue(instance.IsSentiment);
            Assert.IsFalse(instance.IsTopAttribute);
            Assert.AreEqual(2, instance.QuantValue);
            Assert.IsFalse(instance.IsQuestion);
            Assert.IsFalse(instance.IsFeature);
            Assert.IsFalse(instance.IsFixed);
            Assert.IsTrue(instance.IsStopWord);
            Assert.IsTrue(instance.IsSimple);
            Assert.AreEqual(1, instance.AllWords.Count());

            helper.Handler.Verify(item => item.IsFeature(instance), Times.Once);
            helper.Handler.Verify(item => item.IsQuestion(instance), Times.Once);
            helper.AspectDectector.Verify(item => item.IsAttribute(instance), Times.Once);
        }

        [Test]
        public void CreateFixed()
        {
            instance = WordOccurrence.Create(helper.Handler.Object, helper.RawTextExractor.Object, helper.InquirerManager.Object, "xxxbad", null, POSTags.Instance.NN);
            Assert.IsTrue(instance.IsFixed);
        }

        [Test]
        public void Createhashtag()
        {
            instance = WordOccurrence.Create(helper.Handler.Object, helper.RawTextExractor.Object, helper.InquirerManager.Object, "#word", null, POSTags.Instance.NN);
            Assert.AreEqual(NamedEntities.Hashtag, instance.Entity);
            instance.Entity = NamedEntities.Date;
            Assert.AreEqual(NamedEntities.Hashtag, instance.Entity);
        }
    }
}
