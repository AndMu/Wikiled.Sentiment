using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.MachineLearning.Statistics;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Tests.MachineLearning.Statistics
{
    [TestFixture]
    public class SentenceDistributionCalculatorTests
    {
        private Document document;

        [Test]
        public async Task Create()
        {
            await InitDocuments().ConfigureAwait(false);
            SentenceItem sentence = new SentenceItem("Test");
            SentenceDistributionCalculator calculator = new SentenceDistributionCalculator(sentence, document);
            Assert.AreEqual(sentence, calculator.Sentence);
        }

        [Test]
        public async Task GetStatistics1()
        {
            await InitDocuments().ConfigureAwait(false);
            SentenceDistributionCalculator calculator = new SentenceDistributionCalculator(document.Sentences[4], document);
            var result = calculator.GetStatistics(10);
            Assert.AreEqual(33, result.TotalWords);
            Assert.AreEqual(1, result.TotalOccurences);
            Assert.AreEqual(13, result.DataBag.Count());
            Assert.AreEqual(57, result.Statistics.Maximum);
            Assert.AreEqual(17.5, Math.Round(result.Statistics.StandardDeviation, 2));
            Assert.AreEqual(3, result.Statistics.Minimum);
        }

        [Test]
        public async Task GetStatistics2()
        {
            await InitDocuments().ConfigureAwait(false);
            SentenceDistributionCalculator calculator = new SentenceDistributionCalculator(document.Sentences[6], document);
            var result = calculator.GetStatistics(10);
            Assert.AreEqual(33, result.TotalWords);
            Assert.AreEqual(1, result.TotalOccurences);
            Assert.AreEqual(15, result.DataBag.Count());
            Assert.AreEqual(57, result.Statistics.Maximum);
            Assert.AreEqual(19.31, Math.Round(result.Statistics.StandardDeviation, 2));
            Assert.AreEqual(1, result.Statistics.Minimum);
        }

        private async Task InitDocuments()
        {
            if (document != null)
            {
                return;
            }

            document = await ActualWordsHandler.Instance.Loader.InitDocumentWithWords().ConfigureAwait(false);
        }
    }
}
