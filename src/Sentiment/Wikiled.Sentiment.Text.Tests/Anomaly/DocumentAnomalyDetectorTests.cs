using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Anomaly;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.NLP;

namespace Wikiled.Sentiment.Text.Tests.Anomaly
{
    [TestFixture]
    public class DocumentAnomalyDetectorTests
    {
        private IParsedReview document;

        [SetUp]
        public async Task Setup()
        {
            var doc = await ActualWordsHandler.Instance.Loader.InitDocument("cv002_17424.txt").ConfigureAwait(false);
            document = new ParsedReviewFactory(ActualWordsHandler.Instance.WordsHandler, doc).Create();
        }

        [Test]
        public void Measure()
        {
            var distances = new DocumentAnomalyDetector(ActualWordsHandler.Instance.WordsHandler, document);
            distances.Detect();
            Assert.AreEqual(32, document.Sentences.Count);
            Assert.AreEqual(28, distances.WithoutAnomaly.Length);
            Assert.AreEqual(4, distances.Anomaly.Length);
        }

        [Test]
        public void MinimumSentences()
        {
            var distances = new DocumentAnomalyDetector(ActualWordsHandler.Instance.WordsHandler, document);
            distances.Detect();
            Assert.AreEqual(4, distances.MinimumSentencesCount);
            distances.WindowSize = 0.01;
            Assert.AreEqual(1, distances.MinimumSentencesCount);
        }

        [Test]
        public void MinimumWords()
        {
            var distances = new DocumentAnomalyDetector(ActualWordsHandler.Instance.WordsHandler, document);
            distances.Detect();
            Assert.AreEqual(81, distances.MinimumWordsCount);
            distances.WindowSize = 0.01;
            Assert.AreEqual(9, distances.MinimumWordsCount);
            distances.WindowSize = 0.0001;
            Assert.AreEqual(1, distances.MinimumWordsCount);
        }
    }
}
