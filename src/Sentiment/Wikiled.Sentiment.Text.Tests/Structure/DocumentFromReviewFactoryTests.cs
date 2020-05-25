using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Wikiled.Arff.Logic;
using Wikiled.MachineLearning.Mathematics;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Tests.Structure
{
    [TestFixture]
    public class DocumentFromReviewFactoryTests
    {
        private Mock<IParsedReview> review;

        [SetUp]
        public void Setup()
        {
            review = new Mock<IParsedReview>();
        }

        [Test]
        public void ReparseDocument()
        {
            review.Setup(item => item.Text).Returns("DataRow");
            var words = new List<TestWordItem>();
            var sentences = new List<Mock<ISentence>>();
            sentences.Add(new Mock<ISentence>());
            sentences.Add(new Mock<ISentence>());
            foreach (var sentence in sentences)
            {
                sentence.Setup(item => item.Text).Returns("Sentence");
                var currentWords = new List<TestWordItem>();
                for (var i = 0; i < 2; i++)
                {
                    var wordItem = new TestWordItem("Word");
                    wordItem.Relationship = new TestWordItemRelationship();
                    wordItem.Relationship.Sentiment = new SentimentValue(wordItem, "Text", i + 1);
                    currentWords.Add(wordItem);
                    words.Add(wordItem);
                }

                sentence.Setup(item => item.Occurrences).Returns(currentWords.ToArray());
            }

            var adjustment = new Mock<IRatingAdjustment>();
            var rating = new RatingData();
            rating.AddPositive(2);
            adjustment.Setup(item => item.Rating)
                .Returns(rating);

            for (var i = 0; i < words.Count; i++)
            {
                var word = words[i];
                adjustment.Setup(item => item.GetSentiment(word)).Returns(new SentimentValue(word, "Text", 10));
            }

            review.Setup(item => item.Sentences).Returns(sentences.Select(item => item.Object).ToList());
            adjustment.Setup(item => item.Review).Returns(review.Object);
            review.Setup(item => item.Document).Returns(new Document("Test"));
            var document = new DocumentFromReviewFactory().ReparseDocument(adjustment.Object);
            Assert.AreEqual(2, document.Sentences.Count);
            foreach (var sentenceItem in document.Sentences)
            {
                Assert.AreEqual(2, sentenceItem.Words.Count);
                Assert.AreEqual("Sentence", sentenceItem.Text);
                for (var i = 0; i < sentenceItem.Words.Count; i++)
                {
                    Assert.AreEqual(i + 1, sentenceItem.Words[i].Value);
                    Assert.AreEqual(10, sentenceItem.Words[i].CalculatedValue);
                }
            }

            Assert.AreEqual("Test", document.Text);
            Assert.IsTrue(document.GetPositivity() == PositivityType.Positive);
        }
    }
}
