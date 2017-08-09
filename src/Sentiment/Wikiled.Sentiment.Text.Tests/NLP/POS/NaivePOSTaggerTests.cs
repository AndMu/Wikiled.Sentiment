using NUnit.Framework;
using Wikiled.Text.Analysis.POS;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.NLP.Frequency;

namespace Wikiled.Sentiment.Text.Tests.NLP.POS
{
    [TestFixture]
    public class NaivePOSTaggerTests
    {
        private NaivePOSTagger instance;

        [OneTimeSetUp]
        public void Setup()
        {
            instance = new NaivePOSTagger(new BNCList(), WordTypeResolver.Instance);
        }

        [Test]
        public void GetTag()
        {
            var value = instance.GetTag(",");
            Assert.AreEqual(POSTags.Instance.Comma, value);
            value = instance.GetTag("!");
            Assert.AreEqual(POSTags.Instance.SYM, value);
            value = instance.GetTag("axe");
            Assert.AreEqual(POSTags.Instance.NN, value);
            value = instance.GetTag("abject");
            Assert.AreEqual(POSTags.Instance.JJ, value);
            value = instance.GetTag("utterly");
            Assert.AreEqual(POSTags.Instance.RB, value);
            value = instance.GetTag("around");
            Assert.AreEqual(POSTags.Instance.RP, value);
            value = instance.GetTag("xxxx1");
            Assert.AreEqual(POSTags.Instance.UnknownWord, value);
            value = instance.GetTag("a");
            Assert.AreEqual(POSTags.Instance.RP, value);
            value = instance.GetTag("each");
            Assert.AreEqual(POSTags.Instance.PRP, value);
            value = instance.GetTag("run");
            Assert.AreEqual(POSTags.Instance.VB, value);
        }
    }
}
