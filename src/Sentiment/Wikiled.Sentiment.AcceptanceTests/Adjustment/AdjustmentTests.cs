using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.CrossDomain;
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
            var words = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Adjustment\words.csv");
            handler = new BasicWordsHandler(new NaivePOSTagger(null, WordTypeResolver.Instance));
            textSplitter = new SimpleTextSplitter(handler);
            handler.SentimentDataHolder.Clear();
            handler.DisableFeatureSentiment = true;
            var adjuster = new WeightSentimentAdjuster(handler.SentimentDataHolder);
            adjuster.Adjust(words);
        }

        [Test]
        public async Task BasicSentiment()
        {
            var text = ":confused: I do";
            var result = await textSplitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            var review = result.GetReview(handler);
            Assert.AreEqual(1, review.CalculateRawRating().StarsRating);
        }
    }
}
