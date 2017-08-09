using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using Wikiled.Core.Utility.Serialization;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.NLP.Temporal;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.SUTime;

namespace Wikiled.Sentiment.Text.Tests.NLP.Temporal
{
    [TestFixture]
    public class TimeParserTests
    {
        [Test]
        public void GetDimensionById()
        {
            var doc = XDocument.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, @"NLP\Temporal\Cache.xml")).XmlDeserialize<Document>();
            ParsedReviewFactory factory = new ParsedReviewFactory(ActualWordsHandler.Instance.WordsHandler, doc);
            factory.Create();
            var document = new ParsedReviewFactory(ActualWordsHandler.Instance.WordsHandler, doc).Create();

            TimeParser timeParser = new TimeParser(document);
            timeParser.Parse();
            var dimension = timeParser.GetDimensionById(0);
            Assert.AreEqual(TimeDimension.Present, dimension);

            dimension = timeParser.GetDimensionById(4);
            Assert.AreEqual(TimeDimension.Future, dimension);
        }
    }
}
