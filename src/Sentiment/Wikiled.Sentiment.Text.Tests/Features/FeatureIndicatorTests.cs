using System.Linq;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Features;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Text.Tests.Features
{
    [TestFixture]
    public class FeatureIndicatorTests
    {
        private FeatureIndicator instance;

        [OneTimeSetUp]
        public void Setup()
        {
            instance = new FeatureIndicator();
        }

        [TestCase("feature", 0)]
        [TestCase("option", 0)]
        [TestCase("functionality", 0)]
        [TestCase("function", 0)]
        [TestCase("Material", 1)]
        [TestCase("Appearance", 1)]
        [TestCase("Color", 1)]
        [TestCase("Condition", 1)]
        [TestCase("Shape", 1)]
        [TestCase("Size", 1)]
        [TestCase("Sound", 1)]
        [TestCase("Time", 1)]
        [TestCase("Taste", 1)]
        [TestCase("Humidity", 1)]
        [TestCase("Temperature", 1)]
        [TestCase("Touch", 1)]
        [TestCase("Quantity", 1)]
        [TestCase("Material", 1)]
        [TestCase("Cost", 1)]
        [TestCase("structure", 1)]
        [TestCase("good", 1)]
        [TestCase("smell", 1)]
        public void CheckIndication(string word, int blocks)
        {
            var wordItem = ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord(word, POSTags.Instance.NN);
            var result = instance.CheckIndication(wordItem);
            Assert.AreEqual(blocks, result?.Blocks.Count() ?? 0);
        }
    }
}
