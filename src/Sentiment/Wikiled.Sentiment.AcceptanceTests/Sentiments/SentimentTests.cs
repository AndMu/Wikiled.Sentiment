using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Wikiled.Arff.Extensions;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.AcceptanceTests.Helpers;
using Wikiled.Sentiment.AcceptanceTests.Helpers.Data;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Persistency;
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
        private static readonly ILogger log = ApplicationLogging.CreateLogger<SentimentTests>();

        [SetUp]
        public void Setup()
        {
            TestHelper.Instance.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            TestHelper.Instance.Reset();
        }

        [Test]
        public async Task SimpleTest()
        {
            var data = new XmlDataLoader(new NullLogger<XmlDataLoader>()).LoadOldXml(Path.Combine(TestContext.CurrentContext.TestDirectory, "data", "articles.xml"));
            var negative = data.Load()
                               .Where(item => item.Sentiment == SentimentClass.Negative)
                               .Select(item => item.Data)
                               .Repeat(15)
                               .Select(
                item =>
                    {
                        item.Result.Stars = 1;
                        item.Result.Text = item.Result.Text;
                        item.Result.Id = Guid.NewGuid().ToString();
                        return new ParsingDocumentHolder(TestHelper.Instance.ContainerHelper.GetTextSplitter(), item.Result);
                    });

            var positive = data.Load().Where(item => item.Sentiment == SentimentClass.Positive)
                               .Select(item => item.Data)
                               .Repeat(15)
                               .Select(
                                   item =>
                                   {
                                       item.Result.Stars = 5;
                                       item.Result.Text = item.Result.Text;
                                       item.Result.Id = Guid.NewGuid().ToString();
                                       return new ParsingDocumentHolder(TestHelper.Instance.ContainerHelper.GetTextSplitter(), item.Result);
                                   });

            var trainingPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "training");
            var trainingClient = TestHelper.Instance.ContainerHelper.GetTraining(trainingPath);
            await trainingClient.Train(negative.Concat(positive)).ConfigureAwait(false);
            var testingClient = TestHelper.Instance.ContainerHelper.GetTesting(trainingPath);
            testingClient.TrackArff = true;
            testingClient.Init();
            var result = await testingClient.Process(negative.Take(1).Concat(positive.Take(1))).ToArray();
            result = result.OrderBy(item => item.Adjustment.Rating.StarsRating).ToArray();
            Assert.AreEqual(5, result[1].Adjustment.Rating.StarsRating);
            Assert.AreEqual(1, result[0].Adjustment.Rating.StarsRating);
            var classifier = ((MachineSentiment)testingClient.MachineSentiment).Classifier;
            var dataSet = ((MachineSentiment)testingClient.MachineSentiment).DataSet;
            var table = dataSet.GetFeatureTable();
            var invertedSentiment = testingClient.MachineSentiment.GetVector(new[] { new TextVectorCell("NOTxxxexpect", 1) });
            var unknownSentiment = testingClient.MachineSentiment.GetVector(new[] { new TextVectorCell("expect", 1) });
            Assert.AreEqual(unknownSentiment.Vector.Cells[0].Calculated * 4, -invertedSentiment.Vector.Cells[0].Calculated);
            var weights = classifier.Model.ToWeights().Skip(1).ToArray();
        }

        [Test]
        public async Task SimpleAmazonTest()
        {
            log.LogInformation("SimpleTest");
            var reviews = TestHelper.Instance.AmazonRepository.LoadProductReviews("B00005A0QX").ToEnumerable().ToArray();
            var review = reviews.First(item => item.User.Id == "AOJRUSTYHKT1T");
            var doc = await TestHelper.Instance.ContainerHelper.GetTextSplitter().Process(new ParseRequest(review.CreateDocument())).ConfigureAwait(false);
            var result = TestHelper.Instance.ContainerHelper.Resolve<Func<Document, IParsedReviewManager>>()(doc).Create();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Sentences.Count);
            var rating = result.CalculateRawRating();
            Assert.AreEqual(3, rating.StarsRating);
            var words = result.ImportantWords.ToArray();
            Assert.IsTrue(words[0].IsSentiment);
            Assert.IsTrue(words[1].IsSentiment);
            Assert.AreEqual(-1, words[0].Relationship.Sentiment.DataValue.Value);
        }

        [Test]
        public async Task TestNull()
        {
            var doc = await TestHelper.Instance.ContainerHelper.GetTextSplitter().Process(new ParseRequest(new Document())).ConfigureAwait(false);
            Assert.AreEqual(0, doc.Sentences.Count);
        }

        [Test]
        [Ignore("Manually executed only")]
        public async Task FindAnomally()
        {
            log.LogInformation("FindAnomally");
            var data = new SentimentTestData("B00002EQCW");
            var runner = new TestRunner(TestHelper.Instance, data);
            var sentences = await runner.Load()
                .ObserveOn(TaskPoolScheduler.Default)
                .Select(GetSentence)
                .Merge();

            Assert.AreEqual(154, sentences.Length);
        }

        private static async Task<ISentence[]> GetSentence(IParsedDocumentHolder parsedDocument)
        {
            var sentences = new ConcurrentBag<ISentence>();
            var doc = await parsedDocument.GetParsed().ConfigureAwait(false);
            var review = TestHelper.Instance.ContainerHelper.Resolve<Func<Document, IParsedReviewManager>>()(doc).Create();
            foreach (var sentence in review.Sentences)
            {
                var sentiments = sentence.Occurrences.Where(item => item.IsSentiment).ToArray();
                var hasPositive = sentiments.Any(item => item.Relationship.Sentiment.DataValue.IsPositive);
                var hasNegative = sentiments.Any(item => !item.Relationship.Sentiment.DataValue.IsPositive);
                if (hasNegative && hasPositive)
                {
                    sentences.Add(sentence);
                }
            }

            return sentences.ToArray();
        }
    }
}