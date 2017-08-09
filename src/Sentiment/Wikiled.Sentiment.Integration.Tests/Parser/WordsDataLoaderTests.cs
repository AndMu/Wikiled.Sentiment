using NUnit.Framework;
using Wikiled.Sentiment.Integration.Tests.Helpers;
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
            var result = DictionaryHelper.Instance.WordsHandlers.IsQuestion(GetWord(word));
            Assert.AreEqual(expected, result);
        }

        [TestCase("a", true)]
        [TestCase("Some", true)]
        public void IsStop(string word, bool expected)
        {
            var result = DictionaryHelper.Instance.WordsHandlers.IsStop(GetWord(word));
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
            var result = DictionaryHelper.Instance.WordsHandlers.IsSentiment(GetWord(word));
            Assert.AreEqual(expected, result);
        }

        [TestCase("good", true)]
        [TestCase("bad", true)]
        [TestCase("book", false)]
        public void IsKnown(string word, bool expected)
        {
            var result = DictionaryHelper.Instance.WordsHandlers.IsKnown(GetWord(word));
            Assert.AreEqual(expected, result);
        }

        private IWordItem GetWord(string word)
        {
            return DictionaryHelper.Instance.WordsHandlers.WordFactory.CreateWord(word, "NN");
        }
    }
}
