using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Text.Analysis.POS;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;

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
            SimpleTextSplitter splitter = new SimpleTextSplitter(ActualWordsHandler.Instance.WordsHandler);
            ActualWordsHandler.Instance.WordsHandler.AspectDectector = new AspectDectector(new IWordItem[] { }, new IWordItem[] { });
            ActualWordsHandler.Instance.WordsHandler.AspectDectector.AddFeature(ActualWordsHandler.Instance.WordsHandler.WordFactory.CreateWord("teacher", POSTags.Instance.NN));
            var data = await splitter.Process(new ParseRequest("I like my school teacher.")).ConfigureAwait(false);
            review = data.GetReview(ActualWordsHandler.Instance.WordsHandler);
            var sentence = review.Sentences[0];
            instance = new ExtractSentenceTextVector(sentence);
        }

        [TearDown]
        public void Cleanup()
        {
            ActualWordsHandler.Instance.WordsHandler.AspectDectector = NullAspectDectector.Instance;
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
