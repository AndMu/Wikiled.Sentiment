using System;
using System.Threading.Tasks;
using Autofac;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.NLP.NRC;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Text.Tests.MachineLearning
{
    [TestFixture]
    public class ExtractReviewTextVectorTests
    {
        private ExtractReviewTextVector instance;

        private IParsedReview review;

        private Mock<IParsedReview> reviewMock;

        private SimpleTextSplitter splitter;

        [SetUp]
        public void Setup()
        {
            reviewMock = new Mock<IParsedReview>();
            splitter = new SimpleTextSplitter(ActualWordsHandler.Instance.WordsHandler);
            ActualWordsHandler.Instance.WordsHandler.AspectDectector = new AspectDectector(new IWordItem[] {}, new IWordItem[] {});
        }

        [TearDown]
        public void Cleanup()
        {
            ActualWordsHandler.Instance.WordsHandler.AspectDectector = NullAspectDectector.Instance;
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new ExtractReviewTextVector(ActualWordsHandler.Instance.WordsHandler.Container.Resolve<INRCDictionary>(), null));
            Assert.Throws<ArgumentNullException>(() => new ExtractReviewTextVector(null, reviewMock.Object));
        }

        [TestCase(true, true, 2, "my", "like")]
        [TestCase(false, true, 4, "my", "like")]
        [TestCase(true, false, 2, "my", "like")]
        [TestCase(false, false, 5, "my", "teacher")]
        [TestCase(true, true, 4, "not", "NOTxxxxxxFeature")]
        [TestCase(false, true, 6, "not", "NOTxxxxxxFeature")]
        [TestCase(true, false, 2, "not", "NOTxxxlike")]
        [TestCase(false, false, 5, "not", "teacher")]
        public async Task GetCells(bool generate, bool addFeature, int total, string prefix, string lastWord)
        {
            if (addFeature)
            {
                ActualWordsHandler.Instance.WordsHandler.AspectDectector.AddFeature(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("teacher", POSTags.Instance.NN));
            }

            var data = await splitter.Process(new ParseRequest($"I go to school. I like {prefix} teacher.")).ConfigureAwait(false);
            review = new ParsedReviewManager(ActualWordsHandler.Instance.WordsHandler, data).Create();
            instance = new ExtractReviewTextVector(ActualWordsHandler.Instance.WordsHandler.Container.Resolve<INRCDictionary>(), review);
            instance.GenerateUsingImportantOnly = generate;
            var cells = instance.GetCells();
            Assert.AreEqual(total + 1, cells.Count);

            // feature is always 
            Assert.AreEqual(lastWord, cells[cells.Count - 1].Name);
        }
    }
}
