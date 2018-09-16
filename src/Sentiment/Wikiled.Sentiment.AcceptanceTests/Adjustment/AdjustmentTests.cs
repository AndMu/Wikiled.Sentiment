using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sentiment.AcceptanceTests.Helpers;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.AcceptanceTests.Adjustment
{
    [TestFixture]
    public class AdjustmentTests
    {
        private TestHelper testHelper;

        [SetUp]
        public void Setup()
        {
            testHelper = new TestHelper();
        }

        [Test]
        public async Task Adjusted()
        {
            testHelper.ContainerHelper.DataLoader.SentimentDataHolder.Clear();
            testHelper.ContainerHelper.DataLoader.DisableFeatureSentiment = true;
            var adjuster = new WeightSentimentAdjuster(testHelper.ContainerHelper.DataLoader.SentimentDataHolder);
            var words = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Adjustment/words.csv");
            adjuster.Adjust(words);
            var text = "I Veto it";
            var result = await testHelper.ContainerHelper.Splitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            var review = new ParsedReviewManager(testHelper.ContainerHelper.DataLoader, result).Create();
            Assert.AreEqual(1, review.CalculateRawRating().StarsRating);
        }

        [Test]
        public async Task TestEmoticon()
        {
            var text = "EMOTICON_confused I do";
            var result = await testHelper.ContainerHelper.Splitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            var review = new ParsedReviewManager(testHelper.ContainerHelper.DataLoader, result).Create();
            Assert.AreEqual(1, review.CalculateRawRating().StarsRating);
        }
    }
}
