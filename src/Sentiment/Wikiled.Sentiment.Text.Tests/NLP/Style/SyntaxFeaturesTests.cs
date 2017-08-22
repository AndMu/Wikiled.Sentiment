using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.NLP.Style;

namespace Wikiled.Sentiment.Text.Tests.NLP.Style
{
    [TestFixture]
    public class SyntaxFeaturesTests
    {
        private WordsHandlerHelper helper;

        [SetUp]
        public void Setup()
        {
            helper = new WordsHandlerHelper();
        }

        [Test]
        public async Task GetDataFirst()
        {
            var document = await ActualWordsHandler.Instance.Loader.InitDocument().ConfigureAwait(false);
            TextBlock block = new TextBlock(ActualWordsHandler.Instance.WordsHandler, document.Sentences.ToArray());
            Assert.AreEqual(0.0712, Math.Round(block.SyntaxFeatures.AdjectivesPercentage, 4));
            Assert.AreEqual(0.0712, Math.Round(block.SyntaxFeatures.AdverbsPercentage, 4));
            Assert.AreEqual(0.0188, Math.Round(block.SyntaxFeatures.QuestionPercentage, 4));
            Assert.AreEqual(0.164, Math.Round(block.SyntaxFeatures.NounsPercentage, 4));
            Assert.AreEqual(0.1855, Math.Round(block.SyntaxFeatures.VerbsPercentage, 4));
            Assert.AreEqual(0.4344, Math.Round(block.SyntaxFeatures.AdjectivesToNounsRatio, 4));
            Assert.AreEqual(0.0457, Math.Round(block.SyntaxFeatures.ProperNounsPercentage, 4));
            Assert.AreEqual(0, Math.Round(block.SyntaxFeatures.NumbersPercentage, 4));
            Assert.AreEqual(0.7336, Math.Round(block.SyntaxFeatures.POSDiversity, 4));
        }

        [Test]
        public async Task GetDataSecond()
        {
            var document = await ActualWordsHandler.Instance.Loader.InitDocument("cv001_19502.txt").ConfigureAwait(false);
            TextBlock block = new TextBlock(ActualWordsHandler.Instance.WordsHandler, document.Sentences.ToArray());
            Assert.AreEqual(0.0732, Math.Round(block.SyntaxFeatures.AdjectivesPercentage, 4));
            Assert.AreEqual(0.0691, Math.Round(block.SyntaxFeatures.AdverbsPercentage, 4));
            Assert.AreEqual(0.0203, Math.Round(block.SyntaxFeatures.QuestionPercentage, 4));
            Assert.AreEqual(0.2398, Math.Round(block.SyntaxFeatures.NounsPercentage, 4));
            Assert.AreEqual(0.1585, Math.Round(block.SyntaxFeatures.VerbsPercentage, 4));
            Assert.AreEqual(0.3051, Math.Round(block.SyntaxFeatures.AdjectivesToNounsRatio, 4));
            Assert.AreEqual(0.0203, Math.Round(block.SyntaxFeatures.ProperNounsPercentage, 4));
            Assert.AreEqual(0, Math.Round(block.SyntaxFeatures.NumbersPercentage, 4));
            Assert.AreEqual(0.8462, Math.Round(block.SyntaxFeatures.POSDiversity, 4));
        }
    }
}