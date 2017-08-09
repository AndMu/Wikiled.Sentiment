using System.IO;
using NUnit.Framework;
using Wikiled.Sentiment.Integration.Tests.Helpers;
using Wikiled.Sentiment.Text.NLP.Repair;

namespace Wikiled.Sentiment.Integration.Tests.NLP
{
    [TestFixture]
    public class SentenceRepairHandlerTests
    {
        private string repairPath;

        [OneTimeSetUp]
        public void Setup()
        {
            repairPath = Path.Combine(DictionaryHelper.Instance.LibraryPath, "Repair");
        }

        [Test]
        public void RepairNotOnly()
        {
            SentenceRepairHandler handler = new SentenceRepairHandler(repairPath, DictionaryHelper.Instance.WordsHandlers);
            var result = handler.Repair("We not only brought this book but also liked it");
            Assert.AreEqual("We brought this book and liked it", result);
        }

        [Test]
        public void Alot()
        {
            SentenceRepairHandler handler = new SentenceRepairHandler(repairPath, DictionaryHelper.Instance.WordsHandlers);
            var result = handler.Repair("bottom line camera give a lot of bang for buck");
            Assert.AreEqual("bottom line camera give alot bang for buck", result);
        }

        [Test]
        public void RepairEmoticons()
        {
            SentenceRepairHandler handler = new SentenceRepairHandler(repairPath, DictionaryHelper.Instance.WordsHandlers);
            var result = handler.Repair("It was :) but I :(");
            Assert.AreEqual("It was xxxgoodxxxtwo but I xxxbadxxxtwo", result);
        }

        [Test]
        public void RepairSlang()
        {
            SentenceRepairHandler handler = new SentenceRepairHandler(repairPath, DictionaryHelper.Instance.WordsHandlers);
            var result = handler.Repair("It was afaik");
            Assert.AreEqual("It was as far as I know", result);
        }

        [Test]
        public void Enought()
        {
            SentenceRepairHandler handler = new SentenceRepairHandler(repairPath, DictionaryHelper.Instance.WordsHandlers);
            var result = handler.Repair("It is heavy enough to make it stable, but not too heavy to preclude hand use.");
            Assert.AreEqual("It is heavy enough to make it stable, but not too heavy to preclude hand use.", result);
        }

        [Test]
        public void Unison()
        {
            SentenceRepairHandler handler = new SentenceRepairHandler(repairPath, DictionaryHelper.Instance.WordsHandlers);
            var result = handler.Repair("i think this girl unison is nice");
            Assert.AreEqual("i think this girl unison is nice", result);
        }

        [Test]
        public void Non()
        {
            SentenceRepairHandler handler = new SentenceRepairHandler(repairPath, DictionaryHelper.Instance.WordsHandlers);
            var result = handler.Repair("i think non-worthy this girl nonworthy is nice");
            Assert.AreEqual("i think not worthy this girl not worthy is nice", result);
        }
       
    }
}
