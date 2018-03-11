using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Wikiled.Common.Extensions;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Data.Weighting;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Analysis.Tests.Weighting
{
    /// <summary>
    ///     This is a test class for SentimentCalculatorTest and is intended
    ///     to contain all SentimentCalculatorTest Unit Tests
    /// </summary>
    [TestFixture]
    public class SentimentCalculatorTest
    {
        private Mock<IWordItemRelationships> relationships;

        private Mock<IWordItem> wordItem;

        [SetUp]
        public void MyTestInitialize()
        {
            wordItem = new Mock<IWordItem>();
            relationships = new Mock<IWordItemRelationships>();
            wordItem.Setup(item => item.Relationship)
                  .Returns(relationships.Object);
        }

        [Test]
        public void CalculateInvertTest()
        {
            IWordItem invertor = new TestWordItem
            {
                Text = "Invert"
            };

            relationships.Setup(item => item.Inverted)
                         .Returns(invertor);
            relationships.Setup(item => item.PriorQuants)
                         .Returns(new List<IWordItem>());

            SentimentValue value = SentimentValue.CreateGood(wordItem.Object);

            SentimentCalculator target = new SentimentCalculator(value);

            var result = target.Calculate();
            Assert.AreEqual(-1, result.DataValue.Value);
        }

        /// <summary>
        ///     A test for Calculate
        /// </summary>
        [Test]
        public void CalculateQuant()
        {
            relationships.Setup(item => item.Inverted)
                         .Returns((IWordItem)null);
            relationships.Setup(item => item.PriorQuants)
                  .Returns(
                      new List<IWordItem>
                      {
                          new TestWordItem
                          {
                              QuantValue = 10,
                              Text = "Word"
                          }
                      });

            SentimentValue value = SentimentValue.CreateGood(wordItem.Object);
            SentimentCalculator target = new SentimentCalculator(value);

            var result = (double)target.InvokeMethod("CalculateQuant");
            Assert.AreEqual(10, result);
        }

        /// <summary>
        ///     A test for Calculate
        /// </summary>
        [Test]
        public void CalculateQuantInvertor()
        {
            TestWordItem invertor = new TestWordItem();
            relationships.Setup(item => item.Inverted)
                        .Returns(invertor);
            relationships.Setup(item => item.PriorQuants)
                  .Returns(
                      new List<IWordItem>
                      {
                          new TestWordItem
                          {
                              QuantValue = 10,
                              Text = "Word"
                          }
                      });

            SentimentValue value = SentimentValue.CreateGood(wordItem.Object);
            SentimentCalculator target = new SentimentCalculator(value);

            var result = (double)target.InvokeMethod("CalculateQuant");
            Assert.AreEqual(10, result);
        }

        /// <summary>
        ///     A test for Calculate
        /// </summary>
        [Test]
        public void CalculateQuantMultiTest()
        {
            relationships.Setup(item => item.Inverted)
                          .Returns((IWordItem)null);
            relationships.Setup(item => item.PriorQuants)
                  .Returns(
                      new List<IWordItem>
                      {
                          new TestWordItem
                          {
                              QuantValue = 10,
                              Text = "Word"
                          },
                          new TestWordItem
                          {
                              QuantValue = 2,
                              Text = "Word"
                          }
                      });

            SentimentValue value = SentimentValue.CreateGood(wordItem.Object);
            SentimentCalculator target = new SentimentCalculator(value);

            var result = (double)target.InvokeMethod("CalculateQuant");
            Assert.AreEqual(6, result);
        }

        [Test]
        public void CalculateQuantTest()
        {
            relationships.Setup(item => item.Inverted)
                        .Returns((IWordItem)null);
            relationships.Setup(item => item.PriorQuants)
                  .Returns(
                      new List<IWordItem>
                      {
                          new TestWordItem
                          {
                              Text = "Quant",
                              QuantValue = 2
                          }
                      });

            SentimentValue value = SentimentValue.CreateGood(wordItem.Object);
            SentimentCalculator target = new SentimentCalculator(value);

            var result = target.Calculate();
            Assert.AreEqual(2, result.DataValue.Value);
        }

        /// <summary>
        ///     A test for Calculate
        /// </summary>
        [Test]
        public void CalculateQuantZeroTest()
        {
            relationships.Setup(item => item.Inverted)
                         .Returns((IWordItem)null);
            relationships.Setup(item => item.PriorQuants)
                  .Returns(new List<IWordItem>());

            SentimentValue value = SentimentValue.CreateGood(wordItem.Object);
            SentimentCalculator target = new SentimentCalculator(value);

            var result = (double)target.InvokeMethod("CalculateQuant");
            Assert.AreEqual(1, result);
        }

        [Test]
        public void CalculateSmallQuantTest()
        {
            relationships.Setup(item => item.Inverted)
                         .Returns((IWordItem)null);
            relationships.Setup(item => item.PriorQuants)
                  .Returns(
                      new List<IWordItem>
                      {
                          new TestWordItem
                          {
                              Text = "Quant",
                              QuantValue = 0.5
                          }
                      });

            SentimentValue value = SentimentValue.CreateGood(wordItem.Object);
            SentimentCalculator target = new SentimentCalculator(value);
            var result = target.Calculate();
            Assert.AreEqual(0.5, result.DataValue.Value);
        }

        [Test]
        public void SentimentCalculatorConstructorNull2Test()
        {
            Assert.Throws<ArgumentNullException>(() => new SentimentCalculator(null));
        }

        /// <summary>
        ///     A test for SentimentCalculator Constructor
        /// </summary>
        [Test]
        public void SentimentCalculatorConstructorTest()
        {
            IWordItem invertor = new TestWordItem();
            relationships.Setup(item => item.Inverted)
                         .Returns(invertor);
            relationships.Setup(item => item.PriorQuants)
                  .Returns(new List<IWordItem>());

            SentimentValue value = SentimentValue.CreateGood(wordItem.Object);
            SentimentCalculator target = new SentimentCalculator(value);
            Assert.AreEqual(wordItem.Object, target.GetField<IWordItem>("wordItem"));
            Assert.AreEqual(value, target.GetField<SentimentValue>("sentimentValue"));
        }
    }
}
