using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Integration.Tests.Analysis
{
    [TestFixture]
    public class SimpleSentimentAnalysisTests
    {
        private ITextSplitter textSplitter;

        [SetUp]
        public void Setup()
        {
            ActualWordsHandler.InstanceSimple.Reset();
            textSplitter = ActualWordsHandler.InstanceSimple.TextSplitter;
        }

        [TearDown]
        public void Clean()
        {
            ActualWordsHandler.InstanceSimple.Reset();
        }

        [TestCase("I like this pc", 5, 1, false)]
        [TestCase("I hate this pc", 1, 1, false)]
        [TestCase("I don't like to like this kik", 3, 2, false)]
        [TestCase("I don't go to like this kik", 5, 1, true)]
        [TestCase("I don't go to like this kik", 3, 2, false)]
        public async Task TestBasic(string text, int rating, int totalSentiments, bool disableInvertor)
        {
            ActualWordsHandler.InstanceSimple.Container.Context.DisableFeatureSentiment = disableInvertor;
            Document request = await textSplitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            Text.Data.IParsedReview review = ActualWordsHandler.InstanceSimple.Container.Resolve<Func<Document, IParsedReviewManager>>()(request).Create();
            Assert.AreEqual(rating, (int)review.CalculateRawRating().StarsRating);
            SentimentValue[] sentiments = review.GetAllSentiments();
            Assert.AreEqual(totalSentiments, sentiments.Length);
            IRatingAdjustment adjustment = RatingAdjustment.Create(review, new NullMachineSentiment());
            Assert.AreEqual(rating, (int)adjustment.Rating.StarsRating);
        }

        [Test]
        public async Task FullSentence()
        {
            string text = "This tale based on two Edgar Allen Poe pieces (\"The Fall of the House of Usher\", \"Dance of Death\" (poem) ) is actually quite creepy from beginning to end. It is similar to some of the old black-and-white movies about people that meet in an old decrepit house (for example, \"The Cat and the Canary\", \"The Old Dark House\", \"Night of Terror\" and so on). Boris Karloff plays a demented inventor of life-size dolls that terrorize the guests. He dies early in the film (or does he ? ) and the residents of the house are subjected to a number of terrifying experiences. I won't go into too much detail here, but it is definitely a must-see for fans of old dark house mysteries.<br /><br />Watch it with plenty of popcorn and soda in a darkened room.<br /><br />Dan Basinger 8/10";
            ActualWordsHandler.InstanceSimple.Container.Context.DisableFeatureSentiment = true;
            ActualWordsHandler.InstanceSimple.Container.Context.DisableInvertors = true;

            string[] positiveAdj = { "good", "lovely", "excellent", "delightful", "perfect" };
            string[] negativeAdj = { "bad", "horrible", "poor", "disgusting", "unhappy" };
            Dictionary<string, double> sentiment = new Dictionary<string, double>();
            foreach (string item in positiveAdj)
            {
                sentiment[item] = 2;
            }

            foreach (string item in negativeAdj)
            {
                sentiment[item] = -2;
            }

            SentimentDataHolder adjustment = SentimentDataHolder.PopulateEmotionsData(sentiment);
            var request = await textSplitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            ActualWordsHandler.InstanceSimple.Container.Context.Lexicon = adjustment;
            var review = ActualWordsHandler.InstanceSimple.Container.Resolve<Func<Document, IParsedReviewManager>>()(request).Create();

            Assert.IsNull(review.CalculateRawRating().StarsRating);
            SentimentValue[] sentiments = review.GetAllSentiments();
            Assert.AreEqual(0, sentiments.Length);
        }

        [TestCase("like", 2, 1)]
        [TestCase("like", -2, -1)]
        [TestCase("hate", 2, 1)]
        [TestCase("hate", -2, -1)]
        [TestCase("nope", -2, null)]
        public async Task AdjustSentiment(string word, int value, double? rating)
        {
            Document request = await textSplitter.Process(new ParseRequest("Like or hate it")).ConfigureAwait(false);
            var review = ActualWordsHandler.InstanceSimple.Container.Resolve<Func<Document, IParsedReviewManager>>()(request).Create();
            LexiconRatingAdjustment adjustment = new LexiconRatingAdjustment(
                review,
                SentimentDataHolder.Load(new[]
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
            Dictionary<string, double> sentiment = new Dictionary<string, double>();
            sentiment["hate"] = 2;
            SentimentDataHolder adjustment = SentimentDataHolder.PopulateEmotionsData(sentiment);

            Document request = await textSplitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            ActualWordsHandler.InstanceSimple.Container.Context.Lexicon = adjustment;
            var review = ActualWordsHandler.InstanceSimple.Container.Resolve<Func<Document, IParsedReviewManager>>()(request).Create();

            Assert.AreEqual(rating, review.CalculateRawRating().StarsRating);
            SentimentValue[] sentiments = review.GetAllSentiments();
            Assert.AreEqual(totalSentiments, sentiments.Length);
        }
    }
}
