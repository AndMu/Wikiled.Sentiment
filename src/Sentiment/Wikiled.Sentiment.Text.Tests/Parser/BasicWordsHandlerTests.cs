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
            bool value = ActualWordsHandler.InstanceSimple.WordsHandler.IsInvertAdverb(ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("not", "NN"));
            Assert.IsTrue(value);
            
            value = ActualWordsHandler.InstanceSimple.WordsHandler.IsInvertAdverb(ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("nota", "NN"));
            Assert.IsFalse(value);
        }

        [Test]
        public void IsQuestion()
        {
            bool value = ActualWordsHandler.InstanceSimple.WordsHandler.IsQuestion(ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("why", "NN"));
            Assert.IsTrue(value);

            value = ActualWordsHandler.InstanceSimple.WordsHandler.IsInvertAdverb(ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("nota", "NN"));
            Assert.IsFalse(value);
        }

        [Test]
        public void IsKnown()
        {
            bool value = ActualWordsHandler.InstanceSimple.WordsHandler.IsKnown(ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("why", "NN"));
            Assert.IsTrue(value);

            value = ActualWordsHandler.InstanceSimple.WordsHandler.IsKnown(ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("nota", "NN"));
            Assert.IsFalse(value);
        }

        [Test]
        public void IsSentiment()
        {
            bool value = ActualWordsHandler.InstanceSimple.WordsHandler.IsSentiment(ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("why", "NN"));
            Assert.IsFalse(value);
        }

        [Test]
        public void IsStop()
        {
            bool value = ActualWordsHandler.InstanceSimple.WordsHandler.IsStop(ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("it", "NN"));
            Assert.IsTrue(value);

            value = ActualWordsHandler.InstanceSimple.WordsHandler.IsStop(ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("nota", "NN"));
            Assert.IsFalse(value);
        }
    }
}
