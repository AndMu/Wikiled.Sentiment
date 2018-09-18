using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Text.Analysis.Dictionary.Streams;

namespace Wikiled.Sentiment.AcceptanceTests.Sentiments
{
    [TestFixture]
    public class NlpTests
    {
        private string path;

        private IDocumentFromReviewFactory parsedFactory;

        [SetUp]
        public void Setup()
        {
            parsedFactory = new DocumentFromReviewFactory();
            path = Path.Combine(TestContext.CurrentContext.TestDirectory, ConfigurationManager.AppSettings["resources"]);
        }

        [TearDown]
        public void Clean()
        {
            ActualWordsHandler.InstanceOpen.Reset();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task TestReview(bool disableInvert)
        {
            var txt = "#paulryan #killed #rnc2016 #america #died #wisconsin no more EMOTICON_kissing_heart since you gave up on #trump, you don't represent #us";
            var stream = new DictionaryStream(Path.Combine(path, "Library", "Standard", "EmotionLookupTable.txt"), new FileStreamSource());
            var data = stream.ReadDataFromStream(double.Parse).ToDictionary(item => item.Word, item => item.Value, StringComparer.OrdinalIgnoreCase);
            foreach (var item in data.Keys.ToArray().Where(k => !k.StartsWith("EMOTICON")))
            {
                data.Remove(item);
            }

            var lexicon = SentimentDataHolder.PopulateEmotionsData(data);
            ActualWordsHandler.InstanceOpen.Container.Context.DisableInvertors = disableInvert;

            var result = await ActualWordsHandler.InstanceOpen.TextSplitter.Process(new ParseRequest(txt)).ConfigureAwait(false);
            var review = ActualWordsHandler.InstanceOpen.Container.Resolve(result, lexicon).Create();
            var ratings = review.CalculateRawRating();
            Assert.AreEqual(1, review.Sentences.Count);
            Assert.AreEqual(disableInvert, ratings.IsPositive);
        }

        [Test]
        public async Task TestPhrase()
        {
            var result = await ActualWordsHandler.InstanceOpen.TextSplitter.Process(new ParseRequest("In the forest I like perfect dinner")).ConfigureAwait(false);
            var review = ActualWordsHandler.InstanceOpen.Container.Resolve(result).Create();
            Assert.AreEqual(4, review.Items.Count());
        }
    }
}
