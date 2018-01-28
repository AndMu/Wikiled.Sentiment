using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Integration.Tests.Analysis
{
    [TestFixture]
    public class SimpleSentimentAnalysisTests
    {
        private BasicWordsHandler handler;

        private SimpleTextSplitter textSplitter;

        [SetUp]
        public void Setup()
        {
            handler = new BasicWordsHandler(new NaivePOSTagger(null, WordTypeResolver.Instance));
            textSplitter = new SimpleTextSplitter(handler);
        }

        [TestCase("I like this pc", 5, 1, false)]
        [TestCase("I hate this pc", 1, 1, false)]
        [TestCase("I don't like to like this kik", 3, 2, false)]
        [TestCase("I don't go to like this kik", 5, 1, true)]
        [TestCase("I don't go to like this kik", 3, 2, false)]
        public async Task TestBasic(string text, int rating, int totalSentiments, bool disableInvertor)
        {
            handler.DisableFeatureSentiment = disableInvertor;
            var request = await textSplitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            var review = request.GetReview(handler);
            Assert.AreEqual(rating, (int)review.CalculateRawRating().StarsRating);
            var sentiments = review.GetAllSentiments();
            Assert.AreEqual(totalSentiments, sentiments.Length);
        }

        [Test]
        public async Task FullSentece()
        {
            string text = "This tale based on two Edgar Allen Poe pieces (\"The Fall of the House of Usher\", \"Dance of Death\" (poem) ) is actually quite creepy from beginning to end. It is similar to some of the old black-and-white movies about people that meet in an old decrepit house (for example, \"The Cat and the Canary\", \"The Old Dark House\", \"Night of Terror\" and so on). Boris Karloff plays a demented inventor of life-size dolls that terrorize the guests. He dies early in the film (or does he ? ) and the residents of the house are subjected to a number of terrifying experiences. I won't go into too much detail here, but it is definitely a must-see for fans of old dark house mysteries.<br /><br />Watch it with plenty of popcorn and soda in a darkened room.<br /><br />Dan Basinger 8/10";
            handler.DisableFeatureSentiment = true;
            handler.SentimentDataHolder.Clear();
            handler.DisableFeatureSentiment = true;
            var adjuster = new WeightSentimentAdjuster(handler.SentimentDataHolder);

            //adjuster.Adjust(@"e:\Source\PhDDocument\Python\Trump\result.csv");
            var POSITIVE_ADJ = new[] { "good", "lovely", "excellent", "delightful", "perfect" };
            var NEGATIVE_ADJ = new[] { "bad", "horrible", "poor", "disgusting", "unhappy" };
            foreach (var item in POSITIVE_ADJ)
            {
                handler.SentimentDataHolder.SetValue(item, new SentimentValueData(2));
            }

            foreach (var item in NEGATIVE_ADJ)
            {
                handler.SentimentDataHolder.SetValue(item, new SentimentValueData(-2));
            }

            var request = await textSplitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            var review = request.GetReview(handler);
            Assert.IsNull(review.CalculateRawRating().StarsRating);
            var sentiments = review.GetAllSentiments();
            Assert.AreEqual(0, sentiments.Length);
        }

        [TestCase("I like this pc", null, 0)]
        [TestCase("I hate this pc", 5, 1)]
        [TestCase("I don't like to like this kik", 1, 1)]
        public async Task TestCustom(string text, double? rating, int totalSentiments)
        {
            handler.SentimentDataHolder.Clear();
            handler.SentimentDataHolder.SetValue("hate", new SentimentValueData(2));
            var request = await textSplitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            var review = request.GetReview(handler);
            Assert.AreEqual(rating, review.CalculateRawRating().StarsRating);
            var sentiments = review.GetAllSentiments();
            Assert.AreEqual(totalSentiments, sentiments.Length);
        }
    }
}
