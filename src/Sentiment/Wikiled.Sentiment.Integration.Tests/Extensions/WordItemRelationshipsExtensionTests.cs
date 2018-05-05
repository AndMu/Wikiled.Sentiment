using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Extensions;

namespace Wikiled.Sentiment.Integration.Tests.Extensions
{
    [TestFixture]
    public class WordItemRelationshipsExtensionTests
    {
        private List<TestWordItem> items;

        [SetUp]
        public void Setup()
        {
            items = new List<TestWordItem>();
            items.Add(new TestWordItem { Text = "Good", Stemmed = "Good", IsSentiment = true });
            items.Add(new TestWordItem { Text = "Two", Stemmed = "Two" });
            items.Add(new TestWordItem { Text = "#Three", Stemmed = "#Three" });
            items.Add(new TestWordItem { Text = "Four", Stemmed = "Four" });
            for (int i = 1; i < items.Count; i++)
            {
                items[i - 1].Relationship.Next = items[i];
                items[i].Relationship.Previous = items[i - 1];
            }
        }

        [Test]
        public void GetNextNeighbours()
        {
            var result = items[1].Relationship.GetNeighbours(true).ToArray();
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(2, result[1].Item2);
            Assert.AreEqual("Four", result[1].Word.Text);
        }


        [Test]
        public void GetNextNeighboursWithStop()
        {
            items[2].IsStopWord = true;
            var result = items[1].Relationship.GetNeighbours(true).ToArray();
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("Four", result[0].Word.Text);
        }

        [Test]
        public void GetPreviousNeighbours()
        {
            var result = items[1].Relationship.GetNeighbours(false).ToArray();
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(1, result[0].Item2);
        }

        [Test]
        public void GetNext()
        {
            var next = items[1].Relationship.GetNeighbours(true).GetNext();
            Assert.AreEqual(items[2], next);
        }

        [TestCase(3, true, false)]
        [TestCase(3, false, true)]
        [TestCase(2, false, false)]
        public void GetNextByTypes(int resultIndex, bool isFeature, bool isSentiment)
        {
            items[3].IsFeature = isFeature;
            items[3].IsSentiment = isSentiment;
            var next = items[1].Relationship.GetNeighbours(true).GetNext();
            Assert.AreEqual(items[resultIndex], next);
        }

        [Test]
        public void GetNextSntimentOverFeature()
        {
            items[2].IsFeature = true;
            items[3].IsSentiment = true;
            var next = items[1].Relationship.GetNeighbours(true).GetNext();
            Assert.AreEqual(items[3], next);
        }
    }
}