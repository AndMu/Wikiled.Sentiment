using System;
using System.IO;
using System.Runtime.Caching;
using Wikiled.Core.Utility.Resources;
using Wikiled.Sentiment.Text.WordNet.InformationContent;
using Wikiled.Core.Utility.Cache;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.WordNet.Engine;

namespace Wikiled.Sentiment.Analysis.Tests.Wordnet.InformationContent
{
    [TestFixture]
    public class InformationContentResnikTests
    {
        private InformationContentResnik resnik;

        private WordNetEngine engine;
        
        private JcnMeasure instance;

        [OneTimeSetUp]
        public void SetupGlobal()
        {
            var path = new ConfigurationHandler().GetConfiguration("resources");
            resnik = InformationContentResnik.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, path, @"WordNet-InfoContent-3.0\ic-brown-resnik-add1.dat"));
            engine = new WordNetEngine(Path.Combine(TestContext.CurrentContext.TestDirectory, path, @"Wordnet 3.0"));
        }

        [SetUp]
        public void Setup()
        {
            instance = new JcnMeasure(new RuntimeCache(new MemoryCache("Test"), TimeSpan.FromDays(1)), resnik, engine);
        }

        [Test]
        public void JcnSimilar()
        {
            var result = instance.Measure("wheel", "circle");
            Assert.AreEqual(0.36, Math.Round(result, 2));
        }

        [Test]
        public void JcnSimilarGroup()
        {
            var phrase = ActualWordsHandler.Instance.WordsHandler.WordFactory.CreatePhrase("NP");
            phrase.Add(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("Mobile", "NN"));
            phrase.Add(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("device", "NN"));
            var result = instance.Measure(
                phrase,
                ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("Phone", "NN"));
            Assert.AreEqual(0.39, Math.Round(result, 2));
        }

        [Test]
        public void JcnMeasureFork()
        {
            var result = instance.Measure("car", "fork");
            Assert.AreEqual(0.19, Math.Round(result, 2));
        }

        [Test]
        public void JcnMeasureForkSynsets()
        {
            var forkSyn = engine.GetSynSets("fork", WordType.Noun);
            var carSyn = engine.GetSynSets("car", WordType.Noun);
            var result = instance.Measure(forkSyn[4], carSyn[4]);
            Assert.AreEqual(0.17, Math.Round(result, 2));
        }

        [Test]
        public void GetSynSet()
        {
            var value = resnik.GetFrequency(new SynSet(WordType.Noun, 1740));
            Assert.AreEqual(619463.364700726, value);
            value = resnik.GetFrequency(new SynSet(WordType.Noun, 2684));
            Assert.AreEqual(189793.245018072, value);
        }

        [Test]
        public void GetIC()
        {
            var value = resnik.GetIC(new SynSet(WordType.Noun, 1740));
            Assert.AreEqual(0, value);
            value = resnik.GetIC(new SynSet(WordType.Noun, 2684));
            Assert.AreEqual(0.51, Math.Round(value, 2));
        }

        [Test]
        public void JcnMeasure()
        {
            var result = instance.Measure("car", "automobile");
            Assert.AreEqual(1, result);
        }

        [Test]
        public void JcnMeasureWordOccurrence()
        {
            var measure = instance;
            var result = measure.Measure(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("car", "NN"), ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("fork", "NN"));
            Assert.AreEqual(0.19, Math.Round(result, 2));
        }

        [Test]
        public void JcnMeasureWordOccurrenceDifferentType()
        {
            var measure = instance;
            var result = measure.Measure(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("car", "NN"), ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("fork", "JJ"));
            Assert.AreEqual(0, result);
        }

        [Test]
        public void JcnMeasureWordOccurrenceDifferentTypeEnforce()
        {
            var measure = instance;
            var result = measure.Measure(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("car", "NN"), ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("fork", "JJ"), WordType.Noun);
            Assert.AreEqual(0.19, Math.Round(result, 2));
        }

        [Test]
        public void JcnMeasureWordOccurrenceEnforce()
        {
            var measure = instance;
            Assert.Throws<ArgumentOutOfRangeException>(() => measure.Measure(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("car", "NN"), ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("fork", "NN"), WordType.Symbol));
        }

        [Test]
        public void JcnMeasureWordOccurrencePhrase()
        {
            var phrase1 = ActualWordsHandler.Instance.WordsHandler.WordFactory.CreatePhrase("NP");
            phrase1.Add(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("my", "JJ"));
            phrase1.Add(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("car", "NN"));

            var phrase2 = ActualWordsHandler.Instance.WordsHandler.WordFactory.CreatePhrase("NP");
            phrase2.Add(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("his", "JJ"));
            phrase2.Add(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("fork", "NN"));

            var result = instance.Measure(phrase1, phrase2);
            Assert.AreEqual(0.19, Math.Round(result, 2));
        }

    }
}
