using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NLog;
using NUnit.Framework;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.AcceptanceTests.Helpers.Data;
using Wikiled.Sentiment.AcceptanceTests.Helpers;
using Wikiled.Sentiment.Analysis.Amazon;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Extensions;

namespace Wikiled.Sentiment.AcceptanceTests.Sentiments
{
    [TestFixture]
    public class SentimentTest
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        [Test]
        public async Task SimpleTest()
        {
            log.Info("SimpleTest");
            var reviews =  TestHelper.Instance.AmazonRepository.LoadProductReviews("B00005A0QX").ToEnumerable().ToArray();
            var review = reviews.First(item => item.User.Id == "AOJRUSTYHKT1T");
            var result = (await TestHelper.Instance.CachedSplitterHelper.Splitter.Process(new ParseRequest(review.CreateDocument()) {Date = new DateTime(2016, 01, 01)})
                                          .ConfigureAwait(false))
                .GetReview(TestHelper.Instance.CachedSplitterHelper.DataLoader);
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
            var review = doc.GetReview(TestHelper.Instance.CachedSplitterHelper.DataLoader);
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