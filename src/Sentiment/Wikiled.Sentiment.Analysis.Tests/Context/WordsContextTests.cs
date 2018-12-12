using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Context;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Tests.Context
{
    [TestFixture]
    public class WordsContextTests
    {
        private readonly WordEx word = new WordEx();

        [Test]
        public void Create()
        {
            var vectorData = new WordsContext(word);
            Assert.AreEqual(word, vectorData.Word);
            Assert.AreEqual(0, vectorData.Words.Count);
        }

        [Test]
        public void AddContext()
        {
            var addingWord = new WordEx(new SimpleWord("Test"));
            var vectorData = new WordsContext(word);
            vectorData.AddContext(addingWord);
            Assert.AreEqual(1, vectorData.Words.Count);
            vectorData.AddContext(addingWord);
            Assert.AreEqual(2, vectorData.Words.Count);
        }

        [Test]
        public void AddContextSame()
        {
            var vectorData = new WordsContext(word);
            vectorData.AddContext(word);
            Assert.AreEqual(0, vectorData.Words.Count);
        }
    }
}
