using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Context;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Tests.Context
{
    [TestFixture]
    public class WordVectorsDataTests
    {
        private readonly WordEx word = new WordEx();

        [Test]
        public void Constructor()
        {
            var vectorData = new WordVectorsData(word);
            Assert.AreEqual(word, vectorData.Word);
            Assert.AreEqual(0, vectorData.Vectors.Count);
        }

        [Test]
        public void CreateNewVector()
        {
            var vectorData = new WordVectorsData(word);
            Assert.AreEqual(0, vectorData.Vectors.Count);
            Assert.IsNull(vectorData.CurrentVector);
            var vector1 = vectorData.CreateNewVector();
            Assert.IsNotNull(vector1);
            Assert.AreEqual(vector1, vectorData.CurrentVector);
            Assert.AreEqual(1, vectorData.Vectors.Count);
            var vector2 = vectorData.CreateNewVector();
            Assert.AreEqual(1, vectorData.Vectors.Count);
            Assert.AreSame(vector1, vector2);
        }
    }
}
