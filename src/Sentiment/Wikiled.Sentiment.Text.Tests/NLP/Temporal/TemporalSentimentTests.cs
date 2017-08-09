using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using Wikiled.Sentiment.Text.NLP.Temporal.Extractors;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Core.Utility.Serialization;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.NLP.Temporal;

namespace Wikiled.Sentiment.Text.Tests.NLP.Temporal
{
    [TestFixture]
    public class TemporalSentimentTests
    {
        private IParsedReview document;

        [SetUp]
        public void Setup()
        {
            var doc = XDocument.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, @"NLP\Temporal\1.xml")).XmlDeserialize<Document>();
            document = new ParsedReviewFactory(ActualWordsHandler.Instance.WordsHandler, doc).Create();
        }

        [Test]
        public void Parse()
        {
            TemporalSentimentAll process = new TemporalSentimentAll();
            TimeParser parser = new TimeParser(document);
            parser.Parse();
            var result = process.Parse(parser);
            Assert.AreEqual(9, result.Anger);
            Assert.AreEqual(19, result.Anticipation);
            Assert.AreEqual(6, result.Disgust);
            Assert.AreEqual(18, result.Fear);
            Assert.AreEqual(5, result.Joy);
            Assert.AreEqual(15, result.Sadness);
            Assert.AreEqual(5, result.Surprise);
        }
    }
}
