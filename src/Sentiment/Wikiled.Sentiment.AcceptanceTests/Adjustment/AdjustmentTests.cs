using System;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Structure;
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
            var result = await ActualWordsHandler.InstanceOpen.TextSplitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            var document = result.Construct(ActualWordsHandler.InstanceOpen.WordFactory);
            ActualWordsHandler.InstanceOpen.Container.Context.Lexicon = lexicon;
            Text.Data.IParsedReview review = ActualWordsHandler.InstanceOpen.Container.Resolve<Func<Document, IParsedReviewManager>>()(document).Create();
            Assert.AreEqual(1, review.CalculateRawRating().StarsRating);
        }

        [Test]
        public async Task TestEmoticon()
        {
            var text = "EMOTICON_confused I do";
            var result = await ActualWordsHandler.InstanceOpen.TextSplitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            var document = result.Construct(ActualWordsHandler.InstanceOpen.WordFactory);
            var review = ActualWordsHandler.InstanceOpen.Container.Resolve<Func<Document, IParsedReviewManager>>()(document).Create();
            Assert.AreEqual(1, review.CalculateRawRating().StarsRating);
        }

        [Test]
        public async Task Merge()
        {
            ActualWordsHandler.InstanceOpen.Container.Context.DisableFeatureSentiment = true;
            var words = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Adjustment/words.csv");
            ISentimentDataHolder lexicon = SentimentDataHolder.Load(words);

            var loader = SentimentDataHolder.Load(new[] { "veto" }.Select(item =>
                  new WordSentimentValueData(
                      item,
                      new SentimentValueData(2))));

            lexicon.Merge(loader);

            var text = "I Veto it";
            var result = await ActualWordsHandler.InstanceOpen.TextSplitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            var document = result.Construct(ActualWordsHandler.InstanceOpen.WordFactory);
            ActualWordsHandler.InstanceOpen.Container.Context.Lexicon = lexicon;
            Text.Data.IParsedReview review = ActualWordsHandler.InstanceOpen.Container.Resolve<Func<Document, IParsedReviewManager>>()(document).Create();
            Assert.AreEqual(5, review.CalculateRawRating().StarsRating);
        }

        [Test]
        public async Task NgramSentiment()
        {
            ActualWordsHandler.InstanceOpen.Container.Context.DisableFeatureSentiment = true;
            var words = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Adjustment/words.csv");
            ISentimentDataHolder lexicon = SentimentDataHolder.Load(words);

            var loader = SentimentDataHolder.Load(new[] { "veto it really" }.Select(item =>
                new WordSentimentValueData(
                    item,
                    new SentimentValueData(2))));

            lexicon.Merge(loader);

            var text = "I Veto it really";
            var result = await ActualWordsHandler.InstanceOpen.TextSplitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            var document = result.Construct(ActualWordsHandler.InstanceOpen.WordFactory);
            ActualWordsHandler.InstanceOpen.Container.Context.Lexicon = lexicon;
            Text.Data.IParsedReview review = ActualWordsHandler.InstanceOpen.Container.Resolve<Func<Document, IParsedReviewManager>>()(document).Create();
            Assert.AreEqual(1, review.CalculateRawRating().StarsRating);

            ActualWordsHandler.InstanceOpen.Container.Context.NGram = 3;
            review = ActualWordsHandler.InstanceOpen.Container.Resolve<Func<Document, IParsedReviewManager>>()(document).Create();
            Assert.AreEqual(5, review.CalculateRawRating().StarsRating);

            IRatingAdjustment adjustment = RatingAdjustment.Create(review, null);
            var resultDocument = new DocumentFromReviewFactory().ReparseDocument(adjustment);


            Assert.AreEqual(5, resultDocument.Stars);
            Assert.AreEqual("I Veto it really", resultDocument.Text);
        }
    }
}
