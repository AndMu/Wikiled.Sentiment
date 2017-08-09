using NUnit.Framework;
using Wikiled.Sentiment.Text.NLP.Style;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Tests.NLP.Style
{
    [TestFixture]
    public class SentenceItemExtensionTests
    {
        [Test]
        public void IsQuestion()
        {
            bool result = new SentenceItem("Test question").IsQuestion();
            Assert.IsFalse(result);
            result = new SentenceItem("Test question?").IsQuestion();
            Assert.IsTrue(result);
        }

        [Test]
        public void CountPunctuations()
        {
            int result = new SentenceItem("Test question").CountPunctuations();
            Assert.AreEqual(0, result);
            result = new SentenceItem("Test question? Test, 2, 3").CountPunctuations();
            Assert.AreEqual(3, result);
        }

        [Test]
        public void CountCharacters()
        {
            int result = new SentenceItem("Test question").CountCharacters();
            Assert.AreEqual(12, result);
            result = new SentenceItem("Test question?").CountCharacters();
            Assert.AreEqual(12, result);
        }

        [Test]
        public void CountCommas()
        {
            int result = new SentenceItem("Test question").CountCommas();
            Assert.AreEqual(0, result);
            result = new SentenceItem("Test, question?").CountCommas();
            Assert.AreEqual(1, result);
        }

        [Test]
        public void CountSemicolons()
        {
            int result = new SentenceItem("Test question").CountSemicolons();
            Assert.AreEqual(0, result);
            result = new SentenceItem("Test, question;?").CountSemicolons();
            Assert.AreEqual(1, result);
        }
    }
}
