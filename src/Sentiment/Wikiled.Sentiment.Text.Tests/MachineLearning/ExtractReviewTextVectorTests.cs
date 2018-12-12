using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Tests.MachineLearning
{
    [TestFixture]
    public class ExtractReviewTextVectorTests
    {
        private AspectDectector detector;

        private ExtractReviewTextVector instance;

        private IParsedReview review;

        private Mock<IParsedReview> reviewMock;

        private ITextSplitter splitter;

        [SetUp]
        public void Setup()
        {
            reviewMock = new Mock<IParsedReview>();
            splitter = ActualWordsHandler.InstanceSimple.TextSplitter;
            detector = new AspectDectector(new IWordItem[] { }, new IWordItem[] { });
            ActualWordsHandler.InstanceSimple.Container.Context.ChangeAspect(detector);
        }

        [TearDown]
        public void Cleanup()
        {
            ActualWordsHandler.InstanceSimple.Container.Context.ChangeAspect(null);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new ExtractReviewTextVector(ActualWordsHandler.InstanceSimple.Container.Resolve<INRCDictionary>(), null));
            Assert.Throws<ArgumentNullException>(() => new ExtractReviewTextVector(null, reviewMock.Object));
        }

        [TestCase(true, true, 2, "my", "like")]
        [TestCase(false, true, 3, "my", "like")]
        [TestCase(true, false, 2, "my", "like")]
        [TestCase(false, false, 4, "my", "teacher")]
        [TestCase(true, true, 4, "not", "NOTxxxxxxFeature")]
        [TestCase(false, true, 5, "not", "NOTxxxxxxFeature")]
        [TestCase(true, false, 2, "not", "NOTxxxlike")]
        [TestCase(false, false, 4, "not", "teacher")]
        public async Task GetCells(bool generate, bool addFeature, int total, string prefix, string lastWord)
        {
            if (addFeature)
            {
                detector.AddFeature(ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("teacher", POSTags.Instance.NN));
            }

            Document data = await splitter.Process(new ParseRequest($"I go to school. I like {prefix} teacher.")).ConfigureAwait(false);
            review = ActualWordsHandler.InstanceSimple.Container.Resolve<Func<Document, IParsedReviewManager>>()(data).Create();
            instance = new ExtractReviewTextVector(ActualWordsHandler.InstanceSimple.Container.Resolve<INRCDictionary>(), review)
                       {
                           GenerateUsingImportantOnly = generate
                       };
            IList<TextVectorCell> cells = instance.GetCells();
            Assert.AreEqual(total + 1, cells.Count);

            // feature is always 
            Assert.AreEqual(lastWord, cells[cells.Count - 1].Name);
        }
    }
}
