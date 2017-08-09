using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.NLP.Style;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Tests.NLP.Style
{
    [TestFixture]
    public class WordExExtensionTests
    {
        [Test]
        public void CountSyllables()
        {
            WordEx word = WordExFactory.Construct(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("creatures", "NN"));
            Assert.AreEqual(2, word.CountSyllables());

            word = WordExFactory.Construct(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("creature", "NN"));
            Assert.AreEqual(2, word.CountSyllables());
        }
    }
}
