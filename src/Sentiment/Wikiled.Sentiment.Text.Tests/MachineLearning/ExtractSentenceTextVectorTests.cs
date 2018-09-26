using System;
using System.Threading.Tasks;
using Autofac;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Text.Analysis.POS;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Tests.MachineLearning
{
    [TestFixture]
    public class ExtractSentenceTextVectorTests
    {
        private ExtractSentenceTextVector instance;

        private IParsedReview review;

        [SetUp]
        public async Task Setup()
        {
            var aspect = new AspectDectector(new IWordItem[] { }, new IWordItem[] { });
            ActualWordsHandler.InstanceSimple.Container.Context.ChangeAspect(aspect);
            aspect.AddFeature(ActualWordsHandler.InstanceSimple.WordFactory.CreateWord("teacher", POSTags.Instance.NN));
            var data = await ActualWordsHandler.InstanceSimple.TextSplitter.Process(new ParseRequest("I like my school teacher.")).ConfigureAwait(false);
            review = ActualWordsHandler.InstanceSimple.Container.Container.Resolve<Func<Document, IParsedReviewManager>>()(data).Create();
            var sentence = review.Sentences[0];
            instance = new ExtractSentenceTextVector(sentence);
        }

        [TearDown]
        public void Cleanup()
        {
            ActualWordsHandler.InstanceSimple.Reset();
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new ExtractSentenceTextVector(null));
        }

        [TestCase(true, 3)]
        [TestCase(false, 4)]
        public void GetCells(bool generate, int total)
        {
            instance.GenerateUsingImportantOnly = generate;
            var cells = instance.GetCells();
            Assert.AreEqual(total, cells.Count);
            Assert.AreEqual("teacher", cells[cells.Count - 1].Name);
            Assert.AreEqual(1, cells[cells.Count - 1].Value);

            Assert.AreEqual("like", cells[1].Name);
            Assert.AreEqual(2, cells[1].Value);
        }
    }
}
