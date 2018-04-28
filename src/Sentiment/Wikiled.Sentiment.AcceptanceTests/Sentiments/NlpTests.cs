using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.NLP.OpenNLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.Dictionary;
using Wikiled.Text.Analysis.Dictionary.Streams;

namespace Wikiled.Sentiment.AcceptanceTests.Sentiments
{
    [TestFixture]
    public class NlpTests
    {
        private string path;

        private WordsDataLoader wordsHandler;

        private OpenNLPTextSplitter splitter;

        [SetUp]
        public void Setup()
        {
            path = Path.Combine(TestContext.CurrentContext.TestDirectory, ConfigurationManager.AppSettings["resources"]);
            var libraryPath = Path.Combine(path, @"Library/Standard/");
            var dictionary = new BasicEnglishDictionary();
            wordsHandler = new WordsDataLoader(libraryPath, dictionary);
            splitter = new OpenNLPTextSplitter(wordsHandler, path, NullCachedDocumentsSource.Instance);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task TestReview(bool disableInvert)
        {
            var txt = "#paulryan #killed #rnc2016 #america #died #wisconsin no more :kissing_heart: since you gave up on #trump, you don't represent #us";
            var stream = new DictionaryStream(Path.Combine(path, "Library", "Standard", "EmotionLookupTable.txt"), new FileStreamSource());
            var data = stream.ReadDataFromStream(double.Parse).ToDictionary(item => item.Word, item => item.Value, StringComparer.OrdinalIgnoreCase);
            foreach (var item in data.Keys.ToArray().Where(k => !k.StartsWith("EMOTICON")))
            {
                data.Remove(item);
            }

            wordsHandler.SentimentDataHolder.Clear();
            wordsHandler.SentimentDataHolder.PopulateEmotionsData(data);
            wordsHandler.DisableInvertors = disableInvert;


            var result = await splitter.Process(new ParseRequest(txt)).ConfigureAwait(false);
            var review = new ParsedReviewManager(wordsHandler, result).Create();
            var ratings = review.CalculateRawRating();
            Assert.AreEqual(1, review.Sentences.Count);
            Assert.AreEqual(disableInvert, ratings.IsPositive);
        }

        [Test]
        public async Task TestPhrase()
        {
            var result = await splitter.Process(new ParseRequest("In the forest I like perfect dinner")).ConfigureAwait(false);
            ParsedReviewManager factory = new ParsedReviewManager(wordsHandler, result);
            var review = factory.Create();
            Assert.AreEqual(4, review.Items.Count());
        }
    }
}
