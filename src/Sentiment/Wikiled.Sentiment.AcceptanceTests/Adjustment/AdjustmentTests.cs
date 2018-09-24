using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Structure;

namespace Wikiled.Sentiment.AcceptanceTests.Adjustment
{
    [TestFixture]
    public class AdjustmentTests
    {
        [TearDown]
        public void Clean()
        {
            ActualWordsHandler.InstanceOpen.Reset();
        }

        [Test]
        public async Task Adjusted()
        {
            ActualWordsHandler.InstanceOpen.Container.Context.DisableFeatureSentiment = true;
            var words = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Adjustment/words.csv");
            var lexicon = SentimentDataHolder.Load(words);
            var text = "I Veto it";
            var result = await ActualWordsHandler.InstanceOpen.TextSplitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            var review = ActualWordsHandler.InstanceOpen.Container.Resolve(result, lexicon).Create();
            Assert.AreEqual(1, review.CalculateRawRating().StarsRating);
        }

        [Test]
        public async Task TestEmoticon()
        {
            var text = "EMOTICON_confused I do";
            var result = await ActualWordsHandler.InstanceOpen.TextSplitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            var review = ActualWordsHandler.InstanceOpen.Container.Resolve(result).Create();
            Assert.AreEqual(1, review.CalculateRawRating().StarsRating);
        }
    }
}
