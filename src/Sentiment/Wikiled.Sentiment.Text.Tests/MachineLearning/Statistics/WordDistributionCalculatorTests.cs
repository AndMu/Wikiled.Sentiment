using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.MachineLearning.Statistics;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Tests.MachineLearning.Statistics
{
    [TestFixture]
    public class WordDistributionCalculatorTests
    {
        private List<Document> documents;

        [OneTimeSetUp]
        public async Task Setup()
        {
            documents = await ActualWordsHandler.Instance.Loader.InitDocumentsWithWords().ConfigureAwait(false);
        }
        
        [Test]
        public void Create()
        {
            WordEx word = WordExFactory.Construct(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("Movie", "NN"));
            WordDistributionCalculator calculator = new WordDistributionCalculator(word, documents.ToArray());
            Assert.AreEqual(word, calculator.MainItem);
        }

        [Test]
        public void GetWordStatisticsMovie()
        {
            var word = GetWord("Movie");
            WordDistributionCalculator calculator = new WordDistributionCalculator(word, documents.ToArray());
            var result = calculator.GetStatistics(10);
            Assert.AreEqual(9776, result.TotalWords);
            Assert.AreEqual(77, result.TotalOccurences);
            Assert.AreEqual(902, result.DataBag.Count());
            Assert.AreEqual(2, result.Statistics.Maximum);
            Assert.AreEqual(0.5, Math.Round(result.Statistics.StandardDeviation, 2));
            Assert.AreEqual(1, result.Statistics.Minimum);
            Assert.AreEqual(word, calculator.MainItem);
        }

        [Test]
        public void GetWordStatisticsMovies()
        {
            var word = GetWord("Movies");
            WordDistributionCalculator calculator = new WordDistributionCalculator(word, documents.ToArray());
            var result = calculator.GetStatistics(10);
            Assert.AreEqual(9776, result.TotalWords);
            Assert.AreEqual(77, result.TotalOccurences);
            Assert.AreEqual(902, result.DataBag.Count());
            Assert.AreEqual(2, result.Statistics.Maximum);
            Assert.AreEqual(0.5, Math.Round(result.Statistics.StandardDeviation, 2));
            Assert.AreEqual(1, result.Statistics.Minimum);
            Assert.AreEqual(word, calculator.MainItem);
        }

        [Test]
        public void GetWordStatisticsKing()
        {
            var word = GetWord("King");
            WordDistributionCalculator calculator = new WordDistributionCalculator(word, documents.ToArray());
            var result = calculator.GetStatistics(10);
            Assert.AreEqual(9776, result.TotalWords);
            Assert.AreEqual(3, result.TotalOccurences);
            Assert.AreEqual(36, result.DataBag.Count());
            Assert.AreEqual(2, result.Statistics.Maximum);
            Assert.AreEqual(0.51, Math.Round(result.Statistics.StandardDeviation, 2));
            Assert.AreEqual(1, result.Statistics.Minimum);
            Assert.AreEqual(word, calculator.MainItem);
        }

        private WordEx GetWord(string word)
        {
            return WordExFactory.Construct(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord(word, "NN"));
        }
    }
}
