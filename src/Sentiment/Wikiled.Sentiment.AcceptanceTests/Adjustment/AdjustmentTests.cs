using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.AcceptanceTests.Adjustment
{
    [TestFixture]
    public class AdjustmentTests
    {
        private BasicWordsHandler handler;

        private ITextSplitter textSplitter;

        [SetUp]
        public void Setup()
        {
            handler = new BasicWordsHandler(new NaivePOSTagger(null, WordTypeResolver.Instance));
            textSplitter = new SimpleTextSplitter(handler);
        }

        [Test]
        public async Task Adjusted()
        {
            handler.SentimentDataHolder.Clear();
            handler.DisableFeatureSentiment = true;
            var adjuster = new WeightSentimentAdjuster(handler.SentimentDataHolder);
            var words = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Adjustment\words.csv");
            adjuster.Adjust(words);
            var text = "I Veto it";
            var result = await textSplitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            var review = result.GetReview(handler);
            Assert.AreEqual(1, review.CalculateRawRating().StarsRating);
        }

        [Test]
        public async Task BasicSentiment()
        {
            var text = "EMOTICON_confused I do";
            var result = await textSplitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            var review = result.GetReview(handler);
            Assert.AreEqual(1, review.CalculateRawRating().StarsRating);
        }
    }
}
