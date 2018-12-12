using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.Tests.Parser
{
    [TestFixture]
    public class BasicWordsHandlerTests
    {
        [Test]
        public void IsInvertor()
        {
            var value = ActualWordsHandler.InstanceSimple.WordsHandler.IsInvertAdverb(ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("not", "NN"));
            Assert.IsTrue(value);
            
            value = ActualWordsHandler.InstanceSimple.WordsHandler.IsInvertAdverb(ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("nota", "NN"));
            Assert.IsFalse(value);
        }

        [Test]
        public void IsQuestion()
        {
            var value = ActualWordsHandler.InstanceSimple.WordsHandler.IsQuestion(ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("why", "NN"));
            Assert.IsTrue(value);

            value = ActualWordsHandler.InstanceSimple.WordsHandler.IsInvertAdverb(ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("nota", "NN"));
            Assert.IsFalse(value);
        }

        [Test]
        public void IsKnown()
        {
            var value = ActualWordsHandler.InstanceSimple.WordsHandler.IsKnown(ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("why", "NN"));
            Assert.IsTrue(value);

            value = ActualWordsHandler.InstanceSimple.WordsHandler.IsKnown(ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("nota", "NN"));
            Assert.IsFalse(value);
        }

        [Test]
        public void IsSentiment()
        {
            var value = ActualWordsHandler.InstanceSimple.WordsHandler.IsSentiment(ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("why", "NN"));
            Assert.IsFalse(value);
        }

        [Test]
        public void IsStop()
        {
            var value = ActualWordsHandler.InstanceSimple.WordsHandler.IsStop(ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("it", "NN"));
            Assert.IsTrue(value);

            value = ActualWordsHandler.InstanceSimple.WordsHandler.IsStop(ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("nota", "NN"));
            Assert.IsFalse(value);
        }
    }
}
