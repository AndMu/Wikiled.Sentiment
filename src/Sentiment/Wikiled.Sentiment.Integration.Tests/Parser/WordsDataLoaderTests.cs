using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Integration.Tests.Parser
{
    [TestFixture]
    public class WordsDataLoaderTests
    {
        [TestCase("why", true)]
        [TestCase("Some", false)]
        public void IsQuestion(string word, bool expected)
        {
            var result = ActualWordsHandler.InstanceOpen.WordsHandler.IsQuestion(GetWord(word));
            Assert.AreEqual(expected, result);
        }

        [TestCase("a", true)]
        [TestCase("Some", true)]
        public void IsStop(string word, bool expected)
        {
            var result = ActualWordsHandler.InstanceOpen.WordsHandler.IsStop(GetWord(word));
            Assert.AreEqual(expected, result);
        }

        [TestCase("good", true)]
        [TestCase("goods", true)]
        [TestCase("nice", true)]
        [TestCase("bad", true)]
        [TestCase("another", false)]
        [TestCase("Some", false)]
        public void IsSentiment(string word, bool expected)
        {
            var result = ActualWordsHandler.InstanceOpen.WordsHandler.IsSentiment(GetWord(word));
            Assert.AreEqual(expected, result);
        }

        [TestCase("good", true)]
        [TestCase("bad", true)]
        [TestCase("book", false)]
        public void IsKnown(string word, bool expected)
        {
            var result = ActualWordsHandler.InstanceOpen.WordsHandler.IsKnown(GetWord(word));
            Assert.AreEqual(expected, result);
        }

        private IWordItem GetWord(string word)
        {
            return ActualWordsHandler.InstanceOpen.WordFactory.CreateWord(word, "NN");
        }
    }
}
