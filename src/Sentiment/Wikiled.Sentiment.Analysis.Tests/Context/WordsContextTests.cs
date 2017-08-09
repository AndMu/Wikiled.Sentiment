using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Processing.Context;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Tests.Context
{
    [TestFixture]
    public class WordsContextTests
    {
        readonly WordEx word = new WordEx();

        [Test]
        public void Create()
        {
            WordsContext vectorData = new WordsContext(word);
            Assert.AreEqual(word, vectorData.Word);
            Assert.AreEqual(0, vectorData.Words.Count);
        }

        [Test]
        public void AddContext()
        {
            WordEx addingWord = new WordEx(new SimpleWord("Test"));
            WordsContext vectorData = new WordsContext(word);
            vectorData.AddContext(addingWord);
            Assert.AreEqual(1, vectorData.Words.Count);
            vectorData.AddContext(addingWord);
            Assert.AreEqual(2, vectorData.Words.Count);
        }

        [Test]
        public void AddContextSame()
        {
            WordsContext vectorData = new WordsContext(word);
            vectorData.AddContext(word);
            Assert.AreEqual(0, vectorData.Words.Count);
        }
    }
}
