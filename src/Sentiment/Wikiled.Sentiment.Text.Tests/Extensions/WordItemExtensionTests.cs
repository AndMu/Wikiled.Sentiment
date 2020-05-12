using System.Linq;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Tests.Extensions
{
    [TestFixture]
    public class WordItemExtensionTests
    {
        [TestCase("running", true)]
        [TestCase("run", false)]
        public void IsVerbLook(string word, bool expected)
        {
            var wordItem = ActualWordsHandler.InstanceSimple.WordFactory.CreateWord(word, "NN");
            var result = wordItem.IsVerbLook();
            Assert.AreEqual(expected, result);
        }

        [TestCase("emoticon_smile", true)]
        [TestCase("smile", false)]
        public void IsEmoticon(string word, bool expected)
        {
            var wordItem = ActualWordsHandler.InstanceSimple.WordFactory.CreateWord(word, "NN");
            var result = wordItem.IsEmoticon();
            Assert.AreEqual(expected, result);
        }

        [TestCase("a", true)]
        [TestCase("do", true)]
        [TestCase("one", true)]
        [TestCase("his", true)]
        [TestCase("like", false)]
        public void IsNoise(string word, bool expected)
        {
            var wordItem = ActualWordsHandler.InstanceSimple.WordFactory.CreateWord(word, "NN");
            var result = wordItem.IsNoise();
            Assert.AreEqual(expected, result);
        }

        [TestCase("a", NamedEntities.None, true)]
        [TestCase("do", NamedEntities.None, true)]
        [TestCase("one", NamedEntities.None, true)]
        [TestCase("his", NamedEntities.None, true)]
        [TestCase("like", NamedEntities.None, false)]
        [TestCase("a", NamedEntities.Hashtag, false)]
        [TestCase("a", NamedEntities.Person, false)]
        [TestCase("emoticon_", NamedEntities.Hashtag, false)]
        [TestCase("likething", NamedEntities.None, true)]
        public void CanNotBeFeature(string word, NamedEntities entities, bool expected)
        {
            var wordItem = ActualWordsHandler.InstanceSimple.WordFactory.CreateWord(word, "NN");
            wordItem.Entity = entities;
            var result = wordItem.CanNotBeFeature();
            Assert.AreEqual(expected, result);
        }

        [TestCase("a", NamedEntities.None, true)]
        [TestCase("do", NamedEntities.None, true)]
        [TestCase("one", NamedEntities.None, true)]
        [TestCase("his", NamedEntities.None, true)]
        [TestCase("like", NamedEntities.None, false)]
        [TestCase("a", NamedEntities.Hashtag, true)]
        [TestCase("emoticon_", NamedEntities.Hashtag, false)]
        [TestCase("a", NamedEntities.Person, true)]
        [TestCase("likething", NamedEntities.None, false)]
        public void CanNotBeAttribute(string word, NamedEntities entities, bool expected)
        {
            var wordItem = ActualWordsHandler.InstanceSimple.WordFactory.CreateWord(word, "NN");
            wordItem.Entity = entities;
            var result = wordItem.CanNotBeAttribute();
            Assert.AreEqual(expected, result);
        }

        [TestCase("good", "NOTxxxgood")]
        [TestCase("bad", "NOTxxxbad")]
        public void GetInvertedMask(string word, string expected)
        {
            var result = word.GetInvertedMask();
            Assert.AreEqual(expected, result);
        }

        [TestCase("good", "NOTxxxgood")]
        [TestCase("bad", "NOTxxxbad")]
        [TestCase("NOTxxxbad", "bad")]
        [TestCase("NOTxxxgood", "good")]
        public void GetOpposite(string word, string expected)
        {
            var result = word.GetOpposite();
            Assert.AreEqual(expected, result);
        }
    }
}