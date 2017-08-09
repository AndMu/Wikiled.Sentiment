using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.NLP.OpenNLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Persitency;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.Dictionary;
using Wikiled.Text.Analysis.Resources;
using Wikiled.Text.Analysis.WordNet.Engine;

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
            path = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\..\Resources\");
            var libraryPath = Path.Combine(path, @"Library\Standard\");
            var engine = new WordNetEngine(Path.Combine(path, @"Wordnet 3.0"));
            var dictionary = new EnglishDictionary(libraryPath, engine);
            wordsHandler = new WordsDataLoader(libraryPath, dictionary);
            splitter = new OpenNLPTextSplitter(wordsHandler, path, NullCachedDocumentsSource.Instance);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task TestReview(bool disableInvert)
        {
            var txt =
                "#paulryan #killed #rnc2016 #america #died #wisconsin no more :kissing_heart: since you gave up on #trump, you don't represent #us";

            var data = ReadTabResourceDataFile.ReadTextData(Path.Combine(path, "Library", "Standard", "EmotionLookupTable.txt"), false);
            foreach (var item in data.Keys.ToArray().Where(k => !k.StartsWith("EMOTICON")))
            {
                data.Remove(item);
            }

            wordsHandler.SentimentDataHolder.Clear();
            wordsHandler.SentimentDataHolder.PopulateEmotionsData(data);
            wordsHandler.DisableInvertors = disableInvert;


            var result = await splitter.Process(new ParseRequest(txt)).ConfigureAwait(false);
            var review = result.GetReview(wordsHandler);
            var ratings = review.CalculateRawRating();
            Assert.AreEqual(1, review.Sentences.Count);
            Assert.AreEqual(disableInvert, ratings.IsPositive);
        }

        [Test]
        public async Task TestPhrase()
        {
            var result = await splitter.Process(new ParseRequest("In the forest I like perfect dinner")).ConfigureAwait(false);
            ParsedReviewFactory factory = new ParsedReviewFactory(wordsHandler, result);
            var review = factory.Create();
            Assert.AreEqual(4, review.Items.Count());
        }
    }
}
