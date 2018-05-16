using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using MoreLinq;
using NLog;
using NUnit.Framework;
using Wikiled.Arff.Extensions;
using Wikiled.Common.Serialization;
using Wikiled.Sentiment.AcceptanceTests.Helpers;
using Wikiled.Sentiment.AcceptanceTests.Helpers.Data;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Pipeline;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.AcceptanceTests.Sentiments
{
    [TestFixture]
    public class SentimentTests
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        [Test]
        public async Task SimpleTest()
        {
            var doc = XDocument.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, "data", "articles.xml"));
            var data = doc.XmlDeserialize<ProcessingData>();
            var negative = data.Negative.Repeat(5).Select(
                item =>
                    {
                        item.Stars = 1;
                        item.Text = item.Text;
                        item.Id = Guid.NewGuid().ToString();
                        return new ParsingDocumentHolder(TestHelper.Instance.SplitterHelper.Splitter, item);
                    }).ToArray();

            var positive = data.Positive.Repeat(5).Select(
                item =>
                    {
                        item.Stars = 5;
                        item.Text = item.Text;
                        item.Id = Guid.NewGuid().ToString();
                        return new ParsingDocumentHolder(TestHelper.Instance.SplitterHelper.Splitter, item);
                    }).ToArray();


            ProcessingPipeline pipeline = new ProcessingPipeline(TaskPoolScheduler.Default, TestHelper.Instance.SplitterHelper, negative.Union(positive).ToObservable(), new ParsedReviewManagerFactory());
            var trainingPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "training");
            TrainingClient trainingClient = new TrainingClient(pipeline, trainingPath);
            await trainingClient.Train().ConfigureAwait(false);
            pipeline = new ProcessingPipeline(TaskPoolScheduler.Default, TestHelper.Instance.SplitterHelper, negative.Take(1).Union(positive.Take(1)).ToObservable(), new ParsedReviewManagerFactory());
            TestingClient testingClient = new TestingClient(pipeline, trainingPath);
            testingClient.Init();
            var result = await testingClient.Process().ToArray();
            Assert.AreEqual(5, result[0].Adjustment.Rating.StarsRating);
            Assert.AreEqual(1, result[1].Adjustment.Rating.StarsRating);
            var classifier = ((MachineSentiment)testingClient.MachineSentiment).Classifier;
            var dataSet = ((MachineSentiment)testingClient.MachineSentiment).DataSet;
            var table = dataSet.GetFeatureTable();
            //var resultWeight = classifier.Model.Weights[0];
        }

        [Test]
        public async Task SimpleAmazonTest()
        {
            log.Info("SimpleTest");
            var reviews = TestHelper.Instance.AmazonRepository.LoadProductReviews("B00005A0QX").ToEnumerable().ToArray();
            var review = reviews.First(item => item.User.Id == "AOJRUSTYHKT1T");
            var doc = await TestHelper.Instance.SplitterHelper.Splitter.Process(new ParseRequest(review.CreateDocument()) { Date = new DateTime(2016, 01, 01) }).ConfigureAwait(false);
            var result = new ParsedReviewManager(TestHelper.Instance.SplitterHelper.DataLoader, doc).Create();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Sentences.Count);
            var rating = result.CalculateRawRating();
            Assert.AreEqual(3, rating.StarsRating);
            var words = result.Items.ToArray();
            Assert.IsTrue(words[0].IsSentiment);
            Assert.IsTrue(words[1].IsSentiment);
            Assert.AreEqual(-1, words[0].Relationship.Sentiment.DataValue.Value);
        }

        [Test]
        [Ignore("Manually executed only")]
        public async Task FindAnomally()
        {
            log.Info("FindAnomally");
            var data = new SentimentTestData("B00002EQCW");
            TestRunner runner = new TestRunner(TestHelper.Instance, data);
            var sentences = await runner.Load()
                .ObserveOn(TaskPoolScheduler.Default)
                .Select(GetSentence)
                .Merge();

            Assert.AreEqual(154, sentences.Length);
        }

        private static async Task<ISentence[]> GetSentence(IParsedDocumentHolder parsedDocument)
        {
            ConcurrentBag<ISentence> sentences = new ConcurrentBag<ISentence>();
            var doc = await parsedDocument.GetParsed().ConfigureAwait(false);
            var review = new ParsedReviewManager(TestHelper.Instance.SplitterHelper.DataLoader, doc).Create();
            foreach (var sentence in review.Sentences)
            {
                var sentiments = sentence.Occurrences.Where(item => item.IsSentiment).ToArray();
                bool hasPositive = sentiments.Any(item => item.Relationship.Sentiment.DataValue.IsPositive);
                bool hasNegative = sentiments.Any(item => !item.Relationship.Sentiment.DataValue.IsPositive);
                if (hasNegative && hasPositive)
                {
                    sentences.Add(sentence);
                }
            }

            return sentences.ToArray();
        }
    }
}