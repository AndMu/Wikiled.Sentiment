using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;

namespace Wikiled.Sentiment.Text.Tests.Parser
{
    [TestFixture]
    public class BasicWordsHandlerTests
    {
        [Test]
        public void IsInvertor()
        {
            bool value = ActualWordsHandler.Instance.WordsHandler.IsInvertAdverb(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("not", "NN"));
            Assert.IsTrue(value);
            
            value = ActualWordsHandler.Instance.WordsHandler.IsInvertAdverb(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("nota", "NN"));
            Assert.IsFalse(value);
        }

        [Test]
        public void IsQuestion()
        {
            bool value = ActualWordsHandler.Instance.WordsHandler.IsQuestion(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("why", "NN"));
            Assert.IsTrue(value);

            value = ActualWordsHandler.Instance.WordsHandler.IsInvertAdverb(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("nota", "NN"));
            Assert.IsFalse(value);
        }

        [Test]
        public void IsKnown()
        {
            bool value = ActualWordsHandler.Instance.WordsHandler.IsKnown(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("why", "NN"));
            Assert.IsTrue(value);

            value = ActualWordsHandler.Instance.WordsHandler.IsKnown(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("nota", "NN"));
            Assert.IsFalse(value);
        }

        [Test]
        public void IsSentiment()
        {
            bool value = ActualWordsHandler.Instance.WordsHandler.IsSentiment(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("why", "NN"));
            Assert.IsFalse(value);
        }

        [Test]
        public void IsStop()
        {
            bool value = ActualWordsHandler.Instance.WordsHandler.IsStop(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("it", "NN"));
            Assert.IsTrue(value);

            value = ActualWordsHandler.Instance.WordsHandler.IsStop(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("nota", "NN"));
            Assert.IsFalse(value);
        }
    }
}
