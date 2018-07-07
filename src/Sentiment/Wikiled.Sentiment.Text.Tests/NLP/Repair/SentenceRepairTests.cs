using System;
using Moq;
using Wikiled.Sentiment.Text.NLP.Repair;
using NUnit.Framework;
using Wikiled.Text.Analysis.Dictionary;

namespace Wikiled.Sentiment.Text.Tests.NLP.Repair
{
    [TestFixture]
    public class SentenceRepairTests
    {
        private Mock<IWordsDictionary> dictionary;

        [SetUp]
        public void SetUp()
        {
            dictionary = new Mock<IWordsDictionary>();
        }

        [Test]
        public void Create()
        {
            Assert.Throws<ArgumentNullException>(() => new SentenceRepair(null, null, "test", "test"));
            Assert.Throws<ArgumentException>(() => new SentenceRepair(dictionary.Object, "test", null, "test"));
        }

        [Test]
        public void Repair()
        {
            var repair = new SentenceRepair(dictionary.Object, "Test", @"Test(\w+)", "$1");
            var result = repair.Repair("TestWord");
            Assert.AreEqual("Word", result);
        }

        [Test]
        public void RepairNotOnly()
        {
            var repair = new SentenceRepair(dictionary.Object, "Not only", @"(.*) Not only(.*)but also(.*)", "$1$2and$3");
            var result = repair.Repair("We not only brought this book but also liked it");
            Assert.AreEqual("We brought this book and liked it", result);
        }

        [Test]
        public void EasyToUse()
        {
            var repair = new SentenceRepair(dictionary.Object, "easy to use", @"(.*)easy to use(.*)", "$1good$2");
            var result = repair.Repair("It is easy to use at home");
            Assert.AreEqual("It is good at home", result);
        }

        [Test]
        public void Blows()
        {
            var repair = new SentenceRepair(dictionary.Object, "blows", @"(.*)blows(.*) out of( the)? water(.*)", "$1is better then$2$4");
            var result = repair.Repair("it blows all others out of the water ye");
            Assert.AreEqual("it is better then all others ye", result);
        }

        [Test]
        public void Blows2()
        {
            var repair = new SentenceRepair(dictionary.Object, "blows", @"(.*)blows(.*) out of( the)? water(.*)", "$1is better then$2$4");
            var result = repair.Repair("it blows out of the water");
            Assert.AreEqual("it is better then", result);
        }

        [Test]
        public void Blows3()
        {
            var repair = new SentenceRepair(dictionary.Object, "blows", @"(.*)blows(.*) out of( the)? water(.*)", "$1is better then$2$4");
            var result = repair.Repair("it blows out of water");
            Assert.AreEqual("it is better then", result);
        }

        [Test]
        public void Good()
        {
            var repair = new SentenceRepair(dictionary.Object, ":)", @"(.*):\)(.*)", "$1good$2");
            var result = repair.Repair("it :) it");
            Assert.AreEqual("it good it", result);
        }

        [Test]
        public void Bad()
        {
            var repair = new SentenceRepair(dictionary.Object, ":(", @"(.*):\((.*)", "$1bad$2");
            var result = repair.Repair("it :( it");
            Assert.AreEqual("it bad it", result);
        }

        [Test]
        public void Alot()
        {
            var repair = new SentenceRepair(dictionary.Object, "a lot of", @"(.*)a lot of(.*)", "$1alot$2");
            var result = repair.Repair("it is a lot of better");
            Assert.AreEqual("it is alot better", result);
        }

        [Test]
        public void Alot2()
        {
            var repair = new SentenceRepair(dictionary.Object, "a lot", @"(.*)a lot(.*)", "$1alot$2");
            var result = repair.Repair("it is a lot better");
            Assert.AreEqual("it is alot better", result);
        }
    }
}
