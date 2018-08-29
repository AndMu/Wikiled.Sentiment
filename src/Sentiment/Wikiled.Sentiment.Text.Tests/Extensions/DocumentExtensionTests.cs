using System.Collections.Generic;
using NUnit.Framework;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.Structure.Sentiment;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Tests.Extensions
{
    [TestFixture]
    public class DocumentExtensionTests
    {
        private Document document;

        [SetUp]
        public void Setup()
        {
            document = new Document("Test");
            for (int i = 0; i < 3; i++)
            {
                var sentence = new SentenceItem(i.ToString());
                document.Add(sentence);
                for (int j = 0; j < 4; j++)
                {
                    sentence.Add(new WordEx(
                        new SimpleWord(j.ToString()))
                                     {
                                         Value = 2,
                                         CalculatedValue = 3,
                                     });
                }
            }
        }

        [Test]
        public void GetCellsOccurenceOnly()
        {
            IList<TextVectorCell> cells = document.GetCellsOccurenceOnly();
            Assert.AreEqual(4, cells.Count);
            foreach (var textVectorCell in cells)
            {
                Assert.AreEqual(3, textVectorCell.Value);
            }
        }

        [Test]
        public void GetSentimentDataWord()
        {
            var data = document.GetWordSentimentData();
            Assert.AreEqual(12, data.Count);
            Assert.AreEqual(0, data[0].Index);
            Assert.AreEqual(SentimentLevel.Word, data[0].Level);
            Assert.AreEqual("0", data[0].Text);
            Assert.AreEqual(3, data[0].Value);

            Assert.AreEqual(11, data[data.Count - 1].Index);
            Assert.AreEqual(SentimentLevel.Word, data[data.Count - 1].Level);
            Assert.AreEqual("3", data[data.Count - 1].Text);
            Assert.AreEqual(3, data[data.Count - 1].Value);
        }

        [Test]
        public void GetSentenceSentimentData()
        {
            var data = document.GetSentenceSentimentData();
            Assert.AreEqual(3, data.Count);
            Assert.AreEqual(0, data[0].Index);
            Assert.AreEqual(SentimentLevel.Sentence, data[0].Level);
            Assert.AreEqual("0 (Root)", data[0].Text);
            Assert.AreEqual(1, data[0].Value);

            Assert.AreEqual(2, data[2].Index);
            Assert.AreEqual(SentimentLevel.Sentence, data[data.Count - 1].Level);
            Assert.AreEqual("2 (Root)", data[2].Text);
            Assert.AreEqual(1, data[2].Value);
        }
    }
}
