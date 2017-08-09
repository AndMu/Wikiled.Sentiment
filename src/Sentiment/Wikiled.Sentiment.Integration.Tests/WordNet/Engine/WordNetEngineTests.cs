using NUnit.Framework;
using Wikiled.Sentiment.Integration.Tests.Helpers;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.WordNet.Engine;

namespace Wikiled.Sentiment.Integration.Tests.WordNet.Engine
{
    [TestFixture]
    public class WordNetEngineTests
    {
        private IWordNetEngine engine;

        [SetUp]
        public void Setup()
        {
            engine = DictionaryHelper.Instance.Engine;
        }

        [Test]
        public void GetSynSets2()
        {
            
            var synset = engine.GetSynSets("funny");
            Assert.AreEqual(5, synset.Count);
        }

        [Test]
        public void GetSynSets()
        {
            var synset = engine.GetSynSets("fish");
            Assert.AreEqual(6, synset.Count);
        }

        [Test]
        public void GetSynonyms()
        {
            var synonyms = engine.GetSynonyms("bad", WordType.Adjective);
            Assert.AreEqual(45, synonyms.Length);
        }

        [Test]
        public void GetAntonyms()
        {
            var synonyms = engine.GetAntonyms("bad", WordType.Adjective);
            Assert.AreEqual(1, synonyms.Length);
            Assert.AreEqual("good", synonyms[0]);
        }

        [Test]
        public void GetSynSet()
        {
            var synset = engine.GetSynSet(WordType.Noun, 1740);
            Assert.AreEqual(1740, synset.Offset);
            Assert.AreEqual("entity", synset.Words[0]);

            synset = engine.GetSynSet(WordType.Noun, 19128);
            Assert.AreEqual(19128, synset.Offset);
            Assert.AreEqual("natural_object", synset.Words[0]);
        }
    }
}
