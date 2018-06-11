using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Text.Analysis.POS;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Tests.Sentiment
{
    [TestFixture]
    public class ContextSentimentCalculatorTests
    {
        private Mock<IWordItemRelationships> child;

        private Mock<IWordItem> first;

        private Mock<IWordItem> owner;

        private Mock<IWordItemRelationships> parent;

        private Mock<IWordItem> second;

        private Mock<ISentence> sentence;

        private Mock<ISentencePart> sentencePart;

        private Mock<IWordItem> third;

        [SetUp]
        public void Setup()
        {
            sentence = new Mock<ISentence>();
            parent = new Mock<IWordItemRelationships>();
            sentencePart = new Mock<ISentencePart>();
            owner = new Mock<IWordItem>();
            child = new Mock<IWordItemRelationships>();
            parent.Setup(item => item.Owner)
                  .Returns(owner.Object);
            owner.Setup(item => item.WordIndex)
                 .Returns(5);
            first = new Mock<IWordItem>();
            second = new Mock<IWordItem>();
            third = new Mock<IWordItem>();
            sentence.Setup(item => item.Occurrences)
                        .Returns(
                            new[]
                            {
                                first.Object,
                                second.Object,
                                third.Object
                            });

            sentencePart.Setup(item => item.Sentence)
                        .Returns(sentence.Object);
            first.Setup(item => item.POS).Returns(POSTags.Instance.NN);
            first.Setup(item => item.Text).Returns("1");
            first.Setup(item => item.IsSentiment)
                 .Returns(true);
            first.Setup(item => item.IsFeature)
                 .Returns(false);
            first.Setup(item => item.WordIndex)
                 .Returns(1);

            second.Setup(item => item.Text).Returns("2");
            second.Setup(item => item.POS).Returns(POSTags.Instance.NN);
            second.Setup(item => item.IsSentiment)
                  .Returns(false);
            second.Setup(item => item.IsFeature)
                  .Returns(false);
            second.Setup(item => item.WordIndex)
                  .Returns(3);

            third.Setup(item => item.Text).Returns("3");
            third.Setup(item => item.POS).Returns(POSTags.Instance.NN);
            third.Setup(item => item.IsSentiment)
                 .Returns(true);
            third.Setup(item => item.IsFeature)
                 .Returns(false);
            third.Setup(item => item.WordIndex)
                 .Returns(10);

            parent.Setup(item => item.Part)
                  .Returns(sentencePart.Object);

            first.Setup(item => item.Relationship)
                 .Returns(child.Object);
            second.Setup(item => item.Relationship)
                  .Returns(child.Object);
            third.Setup(item => item.Relationship)
                 .Returns(child.Object);

            child.SetupSequence(item => item.Sentiment)
                 .Returns(SentimentValue.CreateBad(owner.Object))
                 .Returns(SentimentValue.CreateGood(owner.Object));
            child.Setup(item => item.PriorQuants)
                 .Returns(new List<IWordItem>());
        }

        [Test]
        public void Process()
        {
            ContextSentimentCalculator calculator = new ContextSentimentCalculator(parent.Object);
            Assert.AreEqual(0, calculator.Sentiments.Count);
            calculator.Process();
            Assert.AreEqual(2, calculator.Sentiments.Count);
            Assert.AreEqual(-0.33, Math.Round(calculator.Sentiments[0].DataValue.Value, 2));
            Assert.AreEqual(0.25, calculator.Sentiments[1].DataValue.Value);
        }

        [Test]
        public void ProcessInvertorNear()
        {
            Mock<IWordItem> invertor = new Mock<IWordItem>();
            child.SetupSequence(item => item.Inverted)
                .Returns((IWordItem)null)
                .Returns(invertor.Object)
                .Returns(invertor.Object);
            invertor.Setup(item => item.WordIndex).Returns(1);

            ContextSentimentCalculator calculator = new ContextSentimentCalculator(parent.Object);
            Assert.AreEqual(0, calculator.Sentiments.Count);
            calculator.Process();
            Assert.AreEqual(2, calculator.Sentiments.Count);
            Assert.AreEqual(-0.33, Math.Round(calculator.Sentiments[0].DataValue.Value, 2));
            Assert.AreEqual(0.33, Math.Round(calculator.Sentiments[1].DataValue.Value, 2));
        }
    }
}
