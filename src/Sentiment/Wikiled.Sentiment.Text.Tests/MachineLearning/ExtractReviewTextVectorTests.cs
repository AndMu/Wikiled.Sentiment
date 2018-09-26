using Autofac;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
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
        private ExtractReviewTextVector instance;

        private IParsedReview review;

        private Mock<IParsedReview> reviewMock;

        private ITextSplitter splitter;

        private AspectDectector detector;

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
            Assert.Throws<ArgumentNullException>(() => new ExtractReviewTextVector(ActualWordsHandler.InstanceSimple.Container.Container.Resolve<INRCDictionary>(), null));
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
                detector.AddFeature(ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("teacher", POSTags.Instance.NN));
            }

            Wikiled.Text.Analysis.Structure.Document data = await splitter.Process(new ParseRequest($"I go to school. I like {prefix} teacher.")).ConfigureAwait(false);
            review = ActualWordsHandler.InstanceSimple.Container.Container.Resolve<Func<Document, IParsedReviewManager>>()(data).Create();
            instance = new ExtractReviewTextVector(ActualWordsHandler.InstanceSimple.Container.Container.Resolve<INRCDictionary>(), review)
            {
                GenerateUsingImportantOnly = generate
            };
            System.Collections.Generic.IList<TextVectorCell> cells = instance.GetCells();
            Assert.AreEqual(total + 1, cells.Count);

            // feature is always 
            Assert.AreEqual(lastWord, cells[cells.Count - 1].Name);
        }
    }
}
