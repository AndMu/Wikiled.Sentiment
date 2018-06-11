using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Integration.Tests.Helpers;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Integration.Tests.Analysis
{
    [TestFixture]
    public class SimpleSentimentAnalysisTests
    {
        private SimpleTextSplitter textSplitter;

        private DictionaryHelper helper;

        [SetUp]
        public void Setup()
        {
            helper = new DictionaryHelper();
            textSplitter = new SimpleTextSplitter(helper.WordsHandlers);
        }

        [TestCase("I like this pc", 5, 1, false)]
        [TestCase("I hate this pc", 1, 1, false)]
        [TestCase("I don't like to like this kik", 3, 2, false)]
        [TestCase("I don't go to like this kik", 5, 1, true)]
        [TestCase("I don't go to like this kik", 3, 2, false)]
        public async Task TestBasic(string text, int rating, int totalSentiments, bool disableInvertor)
        {
            helper.WordsHandlers.DisableFeatureSentiment = disableInvertor;
            var request = await textSplitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            var review = new ParsedReviewManager(helper.WordsHandlers, request).Create();
            Assert.AreEqual(rating, (int)review.CalculateRawRating().StarsRating);
            var sentiments = review.GetAllSentiments();
            Assert.AreEqual(totalSentiments, sentiments.Length);
            var adjustment = RatingAdjustment.Create(review, new NullMachineSentiment());
            Assert.AreEqual(rating, (int)adjustment.Rating.StarsRating);
        }

        [Test]
        public async Task FullSentece()
        {
            string text = "This tale based on two Edgar Allen Poe pieces (\"The Fall of the House of Usher\", \"Dance of Death\" (poem) ) is actually quite creepy from beginning to end. It is similar to some of the old black-and-white movies about people that meet in an old decrepit house (for example, \"The Cat and the Canary\", \"The Old Dark House\", \"Night of Terror\" and so on). Boris Karloff plays a demented inventor of life-size dolls that terrorize the guests. He dies early in the film (or does he ? ) and the residents of the house are subjected to a number of terrifying experiences. I won't go into too much detail here, but it is definitely a must-see for fans of old dark house mysteries.<br /><br />Watch it with plenty of popcorn and soda in a darkened room.<br /><br />Dan Basinger 8/10";
            helper.WordsHandlers.DisableFeatureSentiment = true;
            helper.WordsHandlers.SentimentDataHolder.Clear();
            helper.WordsHandlers.DisableFeatureSentiment = true;

            var positiveAdj = new[] { "good", "lovely", "excellent", "delightful", "perfect" };
            var negativeAdj = new[] { "bad", "horrible", "poor", "disgusting", "unhappy" };
            foreach (var item in positiveAdj)
            {
                helper.WordsHandlers.SentimentDataHolder.SetValue(item, new SentimentValueData(2));
            }

            foreach (var item in negativeAdj)
            {
                helper.WordsHandlers.SentimentDataHolder.SetValue(item, new SentimentValueData(-2));
            }

            var request = await textSplitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            var review = new ParsedReviewManager(helper.WordsHandlers, request).Create();
            Assert.IsNull(review.CalculateRawRating().StarsRating);
            var sentiments = review.GetAllSentiments();
            Assert.AreEqual(0, sentiments.Length);
        }

        [TestCase("like", 2, 1)]
        [TestCase("like", -2, -1)]
        [TestCase("hate", 2, 1)]
        [TestCase("hate", -2, -1)]
        [TestCase("nope", -2, null)]
        public async Task AdjustSentiment(string word, int value, double? rating)
        {
            var request = await textSplitter.Process(new ParseRequest("Like or hate it")).ConfigureAwait(false);
            var review = new ParsedReviewManager(helper.WordsHandlers, request).Create();
            LexiconRatingAdjustment adjustment = new LexiconRatingAdjustment(
                review, 
                SentimentDataHolder.Load(new []
                {
                    new WordSentimentValueData(word, new SentimentValueData(value)),
                }));
            adjustment.CalculateRating();
            Assert.AreEqual(rating, adjustment.Rating.RawRating);
        }

        [TestCase("I like this pc", null, 0)]
        [TestCase("I hate this pc", 5, 1)]
        [TestCase("I don't like to like this kik", 1, 1)]
        public async Task TestCustom(string text, double? rating, int totalSentiments)
        {
            helper.WordsHandlers.SentimentDataHolder.Clear();
            helper.WordsHandlers.SentimentDataHolder.SetValue("hate", new SentimentValueData(2));
            var request = await textSplitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            var review = new ParsedReviewManager(helper.WordsHandlers, request).Create();
            Assert.AreEqual(rating, review.CalculateRawRating().StarsRating);
            var sentiments = review.GetAllSentiments();
            Assert.AreEqual(totalSentiments, sentiments.Length);
        }
    }
}
