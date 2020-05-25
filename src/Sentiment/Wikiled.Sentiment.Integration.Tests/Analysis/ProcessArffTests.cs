using System;
using System.Linq;
using NUnit.Framework;
using Wikiled.Arff.Logic;
using Wikiled.Sentiment.Analysis.Arff;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Integration.Tests.Analysis
{
    [TestFixture]
    public class ProcessArffTests
    {
        private IArffDataSet mockArffDataSet;

        private ProcessArff instance;

        private IParsedReview review;

        private Document document;

        [SetUp]
        public void Setup()
        {
            mockArffDataSet = ArffDataSet.Create<PositivityType>("Test");
            instance = CreateProcessArff();
            document = new Document("Test");
            document.Sentences.Add(new SentenceItem("Test"));
            document.Sentences[0].Words.Add(
                WordExFactory.Construct(
                    new TestWordItem("Good")
                    {
                        Stemmed = "Good",
                        IsSentiment = true
                    }));
            document.Sentences[0].Words.Add(
                WordExFactory.Construct(
                    new TestWordItem("Two")
                    {
                        Stemmed = "Two"
                    }));
            document.Sentences[0].Words.Add(
                WordExFactory.Construct(
                    new TestWordItem("#Three")
                    {
                        Stemmed = "#Three"
                    }));
            var factory = ActualWordsHandler.InstanceSimple.Container.Resolve<Func<Document, IParsedReviewManager>>()(document);
            review = factory.Create();
        }

        [Test]
        public void ParseEmpty()
        {
            document.Sentences.Clear();
            var factory = ActualWordsHandler.InstanceSimple.Container.Resolve<Func<Document, IParsedReviewManager>>()(document);
            review = factory.Create();

            instance.PopulateArff(review, PositivityType.Positive);
            Assert.IsNotNull(instance.DataSet);
            Assert.AreEqual(0, instance.DataSet.Documents.Count());
            Assert.AreEqual(3, instance.DataSet.Header.Total);
        }

        [Test]
        public void ParseSentiment()
        {
            review.ImportantWords.First().Relationship.Sentiment = new SentimentValue(review.ImportantWords.First(), "Text", 1);
            instance.PopulateArff(review, PositivityType.Positive);
            Assert.IsNotNull(instance.DataSet);
            Assert.AreEqual(1, instance.DataSet.Documents.Count());
            Assert.AreEqual(10, instance.DataSet.Header.Total);
        }

        [Test]
        public void ParseHashSentiment()
        {
            review.ImportantWords.First().Relationship.Sentiment = new SentimentValue(review.ImportantWords.First(), "Text", 1);
            var last = review.ImportantWords.Last();
            last.Entity = NamedEntities.Hashtag;
            instance.PopulateArff(review, PositivityType.Positive);
            Assert.IsNotNull(instance.DataSet);
            Assert.AreEqual(1, instance.DataSet.Documents.Count());
            Assert.AreEqual(10, instance.DataSet.Header.Total);
        }

        private ProcessArff CreateProcessArff()
        {
            return new ProcessArff(mockArffDataSet);
        }
    }
}
