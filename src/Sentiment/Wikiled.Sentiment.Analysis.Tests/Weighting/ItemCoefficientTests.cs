using System;
using Wikiled.Sentiment.Text.Data.Weighting;
using NUnit.Framework;

namespace Wikiled.Sentiment.Analysis.Tests.Weighting
{
    [TestFixture]
    public class ItemCoefficientTests
    {
        [Test]
        public void Create()
        {
            ItemCoefficient itemCoefficient = new ItemCoefficient("Test");
            Assert.AreEqual("Test", itemCoefficient.Text);
            Assert.AreEqual(1, itemCoefficient.Value);
        }

        [Test]
        public void CreateNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ItemCoefficient(null));
        }

        [Test]
        public void Readjust()
        {
            ItemCoefficient itemCoefficient = new ItemCoefficient("Test");
            Assert.AreEqual(1, itemCoefficient.Value);
            var newItem = itemCoefficient.Readjust(4);
            Assert.AreEqual(1, itemCoefficient.Value);
            Assert.AreEqual(0.96, newItem.Value);
            newItem = newItem.Readjust(0);
            Assert.AreEqual(1.056, newItem.Value);
            newItem = newItem.Readjust(3);
            Assert.AreEqual(1.024, Math.Round(newItem.Value, 3));
        }

        [Test]
        public void ReadjustTooBig()
        {
            ItemCoefficient itemCoefficient = new ItemCoefficient("Test");
            Assert.Throws<ArgumentOutOfRangeException>(() => itemCoefficient.Readjust(5));
        }
    }
}
