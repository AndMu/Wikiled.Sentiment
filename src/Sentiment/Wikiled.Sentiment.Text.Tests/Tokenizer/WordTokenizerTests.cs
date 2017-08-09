using System.Linq;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Tokenizer;

namespace Wikiled.Sentiment.Text.Tests.Tokenizer
{
    [TestFixture]
    public class WordTokenizerTests
    {
        [Test]
        public void GetWords()
        {
            WordTokenizer tokenizer = new WordTokenizer(
                "Test", 
                new SimpleWordItemFactory(ActualWordsHandler.Instance.WordsHandler), 
                NullSimpleWordPipeline.Instance, 
                NullWordItemPipeline.Instance, 
                new[] { "test", "one" });
            var result = tokenizer.GetWords().ToArray();
            Assert.AreEqual("Test", tokenizer.SentenceText);
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("test", result[0]);
            Assert.AreEqual("one", result[1]);
        }

        [Test]
        public void GetWordsWithFilterOut()
        {
            WordTokenizer tokenizer = new WordTokenizer(
                "Test",
                new SimpleWordItemFactory(ActualWordsHandler.Instance.WordsHandler), 
                NullSimpleWordPipeline.Instance, 
                NullWordItemPipeline.Instance, 
                new[] { "", "one" });
            var result = tokenizer.GetWords().ToArray();
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("one", result[0]);
        }
    }
}
