using System;
using Autofac;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;

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
            ISentimentDataHolder lexicon = SentimentDataHolder.Load(words);
            var text = "I Veto it";
            Document result = await ActualWordsHandler.InstanceOpen.TextSplitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            ActualWordsHandler.InstanceOpen.Container.Context.Lexicon = lexicon;
            Text.Data.IParsedReview review = ActualWordsHandler.InstanceOpen.Container.Resolve<Func<Document, IParsedReviewManager>>()(result).Create();
            Assert.AreEqual(1, review.CalculateRawRating().StarsRating);
        }

        [Test]
        public async Task TestEmoticon()
        {
            var text = "EMOTICON_confused I do";
            Document result = await ActualWordsHandler.InstanceOpen.TextSplitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            var review = ActualWordsHandler.InstanceOpen.Container.Resolve<Func<Document, IParsedReviewManager>>()(result).Create();
            Assert.AreEqual(1, review.CalculateRawRating().StarsRating);
        }
    }
}
