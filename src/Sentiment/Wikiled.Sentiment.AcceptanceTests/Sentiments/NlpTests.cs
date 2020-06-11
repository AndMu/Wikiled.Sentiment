using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Config;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Dictionary.Streams;
using Wikiled.Text.Analysis.Structure;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Utilities.Resources;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Text.Analysis.NLP.NRC;

namespace Wikiled.Sentiment.AcceptanceTests.Sentiments
{
    [TestFixture]
    public class NlpTests
    {
        private string path;

        [SetUp]
        public void Setup()
        {
            var loader = new LexiconConfigLoader(
                ApplicationLogging.LoggerFactory.CreateLogger<LexiconConfigLoader>(),
                new DataDownloader(ApplicationLogging.LoggerFactory.CreateLogger<DataDownloader>()));
            path = loader.Load().Resources;
        }

        [TearDown]
        public void Clean()
        {
            ActualWordsHandler.InstanceOpen.Reset();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task TestReview(bool disableInvert)
        {
            var txt = "#paulryan #killed #rnc2016 #america #died #wisconsin no more EMOTICON_kissing_heart since you gave up on #trump, you don't represent #us";
            var stream = new DictionaryStream(Path.Combine(path, "Library", "Standard", "EmotionLookupTable.txt"), new FileStreamSource());
            var data = stream.ReadDataFromStream(double.Parse).ToDictionary(item => item.Word, item => item.Value, StringComparer.OrdinalIgnoreCase);
            foreach (var item in data.Keys.ToArray().Where(k => !k.StartsWith("EMOTICON")))
            {
                data.Remove(item);
            }

            var lexicon = SentimentDataHolder.PopulateEmotionsData(data);
            ActualWordsHandler.InstanceOpen.Container.Context.DisableInvertors = disableInvert;

            var result = await ActualWordsHandler.InstanceOpen.TextSplitter.Process(new ParseRequest(txt)).ConfigureAwait(false);
            var document = result.Construct(ActualWordsHandler.InstanceOpen.WordFactory);
            ActualWordsHandler.InstanceOpen.Container.Context.Lexicon = lexicon;
            Text.Data.IParsedReview review = ActualWordsHandler.InstanceOpen.Container.Resolve<Func<Document, IParsedReviewManager>>()(document).Create();
            MachineLearning.Mathematics.RatingData ratings = review.CalculateRawRating();
            Assert.AreEqual(1, review.Sentences.Count);
            Assert.AreEqual(disableInvert, ratings.IsPositive);
        }

        [Test]
        public async Task TestPhrase()
        {
            var result = await ActualWordsHandler.InstanceOpen.TextSplitter.Process(new ParseRequest("In the forest I like perfect dinner")).ConfigureAwait(false);
            var document = result.Construct(ActualWordsHandler.InstanceOpen.WordFactory);
            Text.Data.IParsedReview review = ActualWordsHandler.InstanceOpen.Container.Resolve<Func<Document, IParsedReviewManager>>()(document).Create();
            Assert.AreEqual(4, review.ImportantWords.Count());
            Assert.IsNull(document.Attributes);
        }

        [Test]
        public async Task TestAttributes()
        {
            ActualWordsHandler.InstanceOpen.Container.Context.ExtractAttributes = true;
            var result = await ActualWordsHandler.InstanceOpen.TextSplitter.Process(new ParseRequest("In the forest I like perfect dinner")).ConfigureAwait(false);
            var document = result.Construct(ActualWordsHandler.InstanceOpen.WordFactory);
            Text.Data.IParsedReview review = ActualWordsHandler.InstanceOpen.Container.Resolve<Func<Document, IParsedReviewManager>>()(document).Create();
            Assert.AreEqual(4, review.ImportantWords.Count());
            var reparse = new DocumentFromReviewFactory(ActualWordsHandler.InstanceOpen.Container.Resolve<INRCDictionary>());
            IRatingAdjustment adjustment = RatingAdjustment.Create(review, null);
            document = reparse.ReparseDocument(adjustment);
            Assert.AreEqual(8, document.Attributes.Count);
            Assert.AreEqual(7, document.Words.Count(item => item.Attributes.Length > 0));
            Assert.AreEqual(2, document.Words.Count(item => item.Emotions?.Length > 0));
            ActualWordsHandler.InstanceOpen.Container.Context.ExtractAttributes = false;
        }
    }
}
