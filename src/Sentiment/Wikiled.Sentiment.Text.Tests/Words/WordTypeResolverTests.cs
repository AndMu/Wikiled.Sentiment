using NUnit.Framework;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Words;

namespace Wikiled.Sentiment.Text.Tests.Words
{
    [TestFixture]
    public class WordTypeResolverTests
    {
         private IWordTypeResolver instance;

        [OneTimeSetUp]
        public void Setup()
        {
            instance = WordTypeResolver.Instance;
        }

        [Test]
        public void IsSpecialEndSymbol()
        {
            string item = ".";
            Assert.IsFalse(instance.IsSpecialEndSymbol(item));
            item = "...";
            Assert.IsTrue(instance.IsSpecialEndSymbol(item));
            item = "!!";
            Assert.IsTrue(instance.IsSpecialEndSymbol(item));
            item = "??";
            Assert.IsTrue(instance.IsSpecialEndSymbol(item));
        }

        [Test]
        public void IsArticle()
        {
            bool value = instance.IsArticle("a");
            Assert.IsTrue(value);
            value = instance.IsArticle("there");
            Assert.IsFalse(value);
        }

        [Test]
        public void IsConjunction()
        {
            bool value = instance.IsConjunction("and");
            Assert.IsTrue(value);
            value = instance.IsConjunction("there");
            Assert.IsFalse(value);
        }

        [Test]
        public void IsConjunctiveAdverbs()
        {
            bool value = instance.IsConjunctiveAdverbs("incidentally");
            Assert.IsTrue(value);
            value = instance.IsConjunctiveAdverbs("there");
            Assert.IsFalse(value);
        }

        [Test]
        public void IsCoordinatingConjunctions()
        {
            bool value = instance.IsCoordinatingConjunctions("and");
            Assert.IsTrue(value);
            value = instance.IsCoordinatingConjunctions("there");
            Assert.IsFalse(value);
        }

        [Test]
        public void IsInvertingConjunction()
        {
            bool value = instance.IsInvertingConjunction("nevertheless");
            Assert.IsTrue(value);
            value = instance.IsInvertingConjunction("there");
            Assert.IsFalse(value);
        }

        [Test]
        public void IsPreposition()
        {
            bool value = instance.IsPreposition("against");
            Assert.IsTrue(value);
            value = instance.IsPreposition("there");
            Assert.IsFalse(value);
        }

        [Test]
        public void IsPronoun()
        {
            bool value = instance.IsPronoun("hers");
            Assert.IsTrue(value);
            value = instance.IsPronoun("there");
            Assert.IsFalse(value);
        }

        [Test]
        public void IsRegularConjunction()
        {
            bool value = instance.IsRegularConjunction(",");
            Assert.IsTrue(value);
            value = instance.IsRegularConjunction("there");
            Assert.IsFalse(value);
        }

        [Test]
        public void IsSubordinateConjunction()
        {
            bool value = instance.IsSubordinateConjunction("even");
            Assert.IsTrue(value);
            value = instance.IsSubordinateConjunction("there");
            Assert.IsFalse(value);
        }
    }
}
