using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Extensions;

namespace Wikiled.Sentiment.Text.Tests.Extensions
{
    [TestFixture]
    public class WordItemExtensionTests
    {
        [TestCase("running", true)]
        [TestCase("run", false)]
        public void IsVerbLook(string word, bool expected)
        {
            var wordItem = ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord(word, "NN");
            var result = wordItem.IsVerbLook();
            Assert.AreEqual(expected, result);
        }
    }
}
