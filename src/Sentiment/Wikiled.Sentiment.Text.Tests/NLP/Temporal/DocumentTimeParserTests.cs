using System.IO;
using System.Linq;
using System.Xml.Linq;
using NodaTime.Fields;
using NUnit.Framework;
using Wikiled.Core.Utility.Serialization;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.NLP.Temporal;
using Wikiled.Sentiment.Text.Structure;

namespace Wikiled.Sentiment.Text.Tests.NLP.Temporal
{
    [TestFixture]
    public class DocumentTimeParserTests
    {
        [Test]
        public void TestDocument1()
        {
            var doc = XDocument.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, @"NLP\Temporal\1.xml")).XmlDeserialize<Document>();
            var document = new ParsedReviewFactory(ActualWordsHandler.Instance.WordsHandler, doc).Create();
            var parser = new DocumentTimeParser(document);
            parser.Parse();

            Assert.AreEqual(2022, parser.RootBucket.MaximumValue);
            Assert.AreEqual(2012, parser.RootBucket.MinimumValue);
            Assert.IsNull(parser.RootBucket.Granuality);
            Assert.AreEqual(DateTimeFieldType.Year, parser.RootBucket.ZoomGranuality);

            var buckets = parser.RootBucket.Zoomed;
            Assert.AreEqual(6, buckets.Length);
            Assert.AreEqual(1330, buckets[0].MaximumValue);
            Assert.AreEqual(1330, buckets[0].MinimumValue);
            Assert.IsFalse(buckets[0].IsEmpty);
            Assert.AreEqual(1, buckets[0].Words.Count());
            Assert.AreEqual(1, buckets[buckets.Length - 1].Words.Count());
            Assert.AreEqual(1, buckets[buckets.Length - 2].Words.Count());
            
            var zoomed = parser.RootBucket.Zoomed[buckets.Length - 2].Zoomed;
            Assert.AreEqual(12, zoomed.Length);
            Assert.AreEqual(0, zoomed[0].Words.Count());
            Assert.AreEqual(1, zoomed[11].Words.Count());
        }

        [Test]
        public void TestDocument2()
        {
            var doc = XDocument.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, @"NLP\Temporal\2.xml")).XmlDeserialize<Document>();
            var document = new ParsedReviewFactory(ActualWordsHandler.Instance.WordsHandler, doc).Create();
            var parser = new DocumentTimeParser(document);
            parser.Parse();

            Assert.AreEqual(2042, parser.RootBucket.MaximumValue);
            Assert.AreEqual(2003, parser.RootBucket.MinimumValue);
            Assert.IsNull(parser.RootBucket.Granuality);
            Assert.AreEqual(DateTimeFieldType.Year, parser.RootBucket.ZoomGranuality);

            var buckets = parser.RootBucket.Zoomed;
            Assert.AreEqual(9, buckets.Length);
            Assert.AreEqual(12, buckets[1].MaximumValue);
            Assert.AreEqual(1, buckets[1].MinimumValue);
            Assert.AreEqual(2003, buckets[1].Value);
            Assert.IsFalse(buckets[1].IsEmpty);
            Assert.AreEqual(9, buckets[2].Words.Count());
            Assert.IsFalse(buckets[1].IsEmpty);
            Assert.AreEqual(0, buckets[buckets.Length - 1].Words.Count());
            Assert.AreEqual(5, buckets[buckets.Length - 2].Words.Count());

            var zoomed = parser.RootBucket.Zoomed[buckets.Length - 2].Zoomed;
            Assert.AreEqual(12, zoomed.Length);
            Assert.AreEqual(0, zoomed[8].Words.Count());
            Assert.AreEqual(5, zoomed[11].Words.Count());
        }

        [Test]
        public void TestDocument3()
        {
            var doc = XDocument.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, @"NLP\Temporal\3.xml")).XmlDeserialize<Document>();
            var document = new ParsedReviewFactory(ActualWordsHandler.Instance.WordsHandler, doc).Create();
            var parser = new DocumentTimeParser(document);
            parser.Parse();

            Assert.AreEqual(2012, parser.RootBucket.MaximumValue);
            Assert.AreEqual(2012, parser.RootBucket.MinimumValue);
            Assert.IsNull(parser.RootBucket.Granuality);
            Assert.AreEqual(DateTimeFieldType.Year, parser.RootBucket.ZoomGranuality);

            var buckets = parser.RootBucket.Zoomed;
            Assert.AreEqual(3, buckets.Length);
            Assert.AreEqual(11, buckets[1].MaximumValue);
            Assert.AreEqual(11, buckets[1].MinimumValue);
            Assert.IsFalse(buckets[1].IsEmpty);
            Assert.AreEqual(1, buckets[1].Words.Count());
        }

        [Test]
        public void TestDocument4()
        {
            var doc = XDocument.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, @"NLP\Temporal\4.xml")).XmlDeserialize<Document>();
            var document = new ParsedReviewFactory(ActualWordsHandler.Instance.WordsHandler, doc).Create();
            DocumentTimeParser parser = new DocumentTimeParser(document);
            parser.Parse();
        }

        [Test]
        public void TestDocumentEmpty()
        {
            DocumentTimeParser parser = new DocumentTimeParser(new ParsedReviewFactory(ActualWordsHandler.Instance.WordsHandler, new Document()).Create());
            parser.Parse();
            Assert.AreEqual(0, parser.RootBucket.MaximumValue);
            Assert.AreEqual(0, parser.RootBucket.MinimumValue);
            Assert.IsNull(parser.RootBucket.Granuality);
        }
    }
}