using System;
using NUnit.Framework;
using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Analysis.Tests.Sentiments
{
    [TestFixture]
    public class RatingCalculatorTests
    {
        [Test]
        public void ConvertToRaw()
        {
            Assert.AreEqual(4, RatingCalculator.CalculateStar(0.5));
            Assert.AreEqual(0.5, RatingCalculator.ConvertToRaw(4));
            Assert.AreEqual(-1, RatingCalculator.ConvertToRaw(1));
        }

        [Test]
        public void Calculate()
        {
            var value = RatingCalculator.Calculate(10, 0);
            Assert.AreEqual(1, value);

            value = RatingCalculator.Calculate(10, 10);
            Assert.AreEqual(0, value);

            value = RatingCalculator.Calculate(0, 10);
            Assert.AreEqual(-1, value);

            value = RatingCalculator.Calculate(1, 3);
            Assert.AreEqual(-0.792, Math.Round(value, 3));

            value = RatingCalculator.Calculate(3, 1);
            Assert.AreEqual(0.792, Math.Round(value, 3));

            value = RatingCalculator.Calculate(1000, 1);
            Assert.AreEqual(1, value);
        }

        [Test]
        public void CalculateStar()
        {
            Assert.AreEqual(1, RatingCalculator.CalculateStar(-1));
            Assert.AreEqual(5, RatingCalculator.CalculateStar(1));
            Assert.IsNull(RatingCalculator.CalculateStar(null));
            Assert.AreEqual(3, RatingCalculator.CalculateStar(0));
            Assert.AreEqual(4, RatingCalculator.CalculateStar(0.5));
            Assert.AreEqual(2, RatingCalculator.CalculateStar(-0.5));
        }
    }
}
