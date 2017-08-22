using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Text.Analysis.NLP.NRC;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Tests.NLP.NRC
{
    [TestFixture]
    public class NRCDictionaryExtensionTests
    {
        private Mock<INRCDictionary> dictionary;

        [SetUp]
        public void Setup()
        {
            dictionary = new Mock<INRCDictionary>();
            dictionary.Setup(item => item.FindRecord("kill"))
                      .Returns(
                          new NRCRecord
                          {
                              IsFear = true,
                              IsSadness = true
                          });

            dictionary.Setup(item => item.FindRecord("love"))
                      .Returns(
                          new NRCRecord
                          {
                              IsJoy = true
                          });
        }

        [Test]
        public void TestInverted()
        {
            var record = dictionary.Object.FindRecord(
                new TestWordItem
                {
                    Text = "kill",
                    Relationship = new TestWordItemRelationship
                                   {
                                       Inverted = new TestWordItem()
                                   }
                });
            Assert.IsTrue(record.IsAnger);
            Assert.IsFalse(record.IsAnticipation);
            Assert.IsFalse(record.IsDisgust);
            Assert.IsFalse(record.IsFear);
            Assert.IsTrue(record.IsJoy);
            Assert.IsFalse(record.IsNegative);
            Assert.IsFalse(record.IsPositive);
            Assert.IsFalse(record.IsSadness);
            Assert.IsFalse(record.IsSurprise);
            Assert.IsFalse(record.IsTrust);
        }

        [Test]
        public void Extract()
        {
            var vector = dictionary.Object.Extract(new[] { new WordEx(new SimpleWord("kill")) });
            Assert.AreEqual(0, vector.Anger);
            Assert.AreEqual(0, vector.Anticipation);
            Assert.AreEqual(0, vector.Disgust);
            Assert.AreEqual(1, vector.Fear);
            Assert.AreEqual(0, vector.Joy);
            Assert.AreEqual(0, vector.Trust);
            Assert.AreEqual(1, vector.Sadness);
            Assert.AreEqual(0, vector.Surprise);
            Assert.AreEqual(1, vector.Total);
            Assert.AreEqual(2, vector.TotalSum);

            vector = dictionary.Object.Extract(new[] { new WordEx(new SimpleWord("kill")) });
            Assert.AreEqual(0, vector.Anger);
            Assert.AreEqual(0, vector.Anticipation);
            Assert.AreEqual(0, vector.Disgust);
            Assert.AreEqual(2, vector.Fear);
            Assert.AreEqual(0, vector.Joy);
            Assert.AreEqual(0, vector.Trust);
            Assert.AreEqual(2, vector.Sadness);
            Assert.AreEqual(0, vector.Surprise);
            Assert.AreEqual(2, vector.Total);
            Assert.AreEqual(4, vector.TotalSum);

            vector = dictionary.Object.Extract(new[] { new WordEx(new SimpleWord("love")) });
            Assert.AreEqual(0, vector.Anger);
            Assert.AreEqual(0, vector.Anticipation);
            Assert.AreEqual(0, vector.Disgust);
            Assert.AreEqual(2, vector.Fear);
            Assert.AreEqual(1, vector.Joy);
            Assert.AreEqual(2, vector.Sadness);
            Assert.AreEqual(0, vector.Surprise);
            Assert.AreEqual(0, vector.Trust);
            Assert.AreEqual(3, vector.Total);
            Assert.AreEqual(5, vector.TotalSum);
        }

        [Test]
        public void GetOccurences()
        {
            SentimentVector vector = new SentimentVector();
            dictionary.Object.ExtractToVector(vector, new[] { new TestWordItem { Text = "kill" } });
            dictionary.Object.ExtractToVector(vector, new[] { new TestWordItem { Text = "kill" } });
            dictionary.Object.ExtractToVector(vector, new[] { new TestWordItem { Text = "love" } });
            var data = vector.GetOccurences().ToArray();
            Assert.AreEqual(8, data.Length);
            Assert.AreEqual("Anger", data[0].Data);
            Assert.AreEqual(0, data[0].Probability);
            Assert.AreEqual("Fear", data[3].Data);
            Assert.AreEqual(2, data[3].Probability);
            Assert.AreEqual("Sadness", data[5].Data);
            Assert.AreEqual(2, data[5].Probability);
            Assert.AreEqual("Joy", data[4].Data);
            Assert.AreEqual(1, data[4].Probability);
        }

        [Test]
        public void GetProbabilities()
        {
            SentimentVector vector = new SentimentVector();
            dictionary.Object.ExtractToVector(vector, new[] { new TestWordItem { Text = "kill" } });
            dictionary.Object.ExtractToVector(vector, new[] { new TestWordItem { Text = "kill" } });
            dictionary.Object.ExtractToVector(vector, new[] { new TestWordItem { Text = "love" } });
            var data = vector.GetProbabilities().ToArray();
            Assert.AreEqual(8, data.Length);
            Assert.AreEqual("Anger", data[0].Data);
            Assert.AreEqual(0, data[0].Probability);
            Assert.AreEqual("Fear", data[3].Data);
            Assert.AreEqual(0.67, Math.Round(data[3].Probability, 2));
            Assert.AreEqual("Sadness", data[5].Data);
            Assert.AreEqual(0.67, Math.Round(data[5].Probability, 2));
            Assert.AreEqual("Joy", data[4].Data);
            Assert.AreEqual(0.33, Math.Round(data[4].Probability, 2));
        }
    }
}

