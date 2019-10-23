using System;
using NUnit.Framework;
using System.Threading.Tasks;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Integration.Tests.Analysis
{
    [TestFixture]
    public class OpenNlpSentimentAnalysisTests
    {
        private ITextSplitter textSplitter;

        [SetUp]
        public void Setup()
        {
            ActualWordsHandler.InstanceOpen.Reset();
            textSplitter = ActualWordsHandler.InstanceOpen.TextSplitter;
        }

        [TearDown]
        public void Clean()
        {
            ActualWordsHandler.InstanceOpen.Reset();
        }

        [TestCase("I like this pc", 5, 1, false)]
        [TestCase("I hate this pc", 1, 1, false)]
        [TestCase("I don't like to like this kik", 3, 2, false)]
        [TestCase("I don't go to like this kik", 5, 1, true)]
        [TestCase("I don't go to like this kik", 3, 2, false)]
        [TestCase("Tom :) go for it", 5, 1, false)]
        [TestCase("Tom wat up", 5, 1, false)]
        public async Task TestBasic(string text, int rating, int totalSentiments, bool disableInvertor)
        {
            ActualWordsHandler.InstanceOpen.Container.Context.DisableFeatureSentiment = disableInvertor;
            var request = await textSplitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            var document = request.Construct(ActualWordsHandler.InstanceOpen.WordFactory);
            Text.Data.IParsedReview review = ActualWordsHandler.InstanceOpen.Container.Resolve<Func<Document, IParsedReviewManager>>()(document).Create();
            Assert.AreEqual(rating, (int)review.CalculateRawRating().StarsRating);
            SentimentValue[] sentiments = review.GetAllSentiments();
            Assert.AreEqual(totalSentiments, sentiments.Length);
            IRatingAdjustment adjustment = RatingAdjustment.Create(review, new NullMachineSentiment());
            Assert.AreEqual(rating, (int)adjustment.Rating.StarsRating);
        }
    }
}
