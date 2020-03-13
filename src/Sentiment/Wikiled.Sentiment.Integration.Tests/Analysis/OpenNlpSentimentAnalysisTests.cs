using System;
using System.Linq;
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
        [TestCase("Tom :) up", 5, 1, false)]
        public async Task TestBasic(string text, int rating, int totalSentiments, bool disableInvertor)
        {
            ActualWordsHandler.InstanceOpen.Container.Context.DisableFeatureSentiment = disableInvertor;
            var request = await textSplitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            var document = request.Construct(ActualWordsHandler.InstanceOpen.WordFactory);
            Text.Data.IParsedReview review =
                ActualWordsHandler.InstanceOpen.Container.Resolve<Func<Document, IParsedReviewManager>>()(document)
                    .Create();
            Assert.AreEqual(rating, (int) review.CalculateRawRating().StarsRating);
            SentimentValue[] sentiments = review.GetAllSentiments();
            Assert.AreEqual(totalSentiments, sentiments.Length);
            IRatingAdjustment adjustment = RatingAdjustment.Create(review, new NullMachineSentiment());
            Assert.AreEqual(rating, (int) adjustment.Rating.StarsRating);
        }

        [Test]
        public async Task FullSentence()
        {
            var text =
                "Today, this tale based on two Edgar Allen Poe pieces (\"The Fall of the House of Usher\", \"Dance of Death\" (poem) ) is actually quite creepy from beginning to end. It is similar to some of the old black-and-white movies about people that meet in an old decrepit house (for example, \"The Cat and the Canary\", \"The Old Dark House\", \"Night of Terror\" and so on). Boris Karloff plays a demented inventor of life-size dolls that terrorize the guests. He dies early in the film (or does he ? ) and the residents of the house are subjected to a number of terrifying experiences. I won't go into too much detail here, but it is definitely a must-see for fans of old dark house mysteries.<br /><br />Watch it with plenty of popcorn and soda in a darkened room.<br /><br />Dan Basinger 8/10";
            var request = await textSplitter.Process(new ParseRequest(text)).ConfigureAwait(false);
            var document = request.Construct(ActualWordsHandler.InstanceSimple.WordFactory);
            Text.Data.IParsedReview review =
                ActualWordsHandler.InstanceOpen.Container.Resolve<Func<Document, IParsedReviewManager>>()(document)
                    .Create();
            Assert.AreEqual(1, (int)review.CalculateRawRating().StarsRating);
            Assert.AreEqual(10, document.Words.Count(item => item.EntityType != NamedEntities.None));
        }

        [Test]
        public async Task TestCustomNer()
        {
            var request = await textSplitter.Process(new ParseRequest("We love XXX!")).ConfigureAwait(false);
            var document = request.Construct(ActualWordsHandler.InstanceOpen.WordFactory);
            Text.Data.IParsedReview review =
                ActualWordsHandler.InstanceOpen.Container.Resolve<Func<Document, IParsedReviewManager>>()(document)
                    .Create();
            Assert.AreEqual("XXX", document.Words.ToArray()[2].CustomEntity);
        }
    }
}
