using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Tests.Processing
{
    [TestFixture]
    public class ProcessingDataExtensionTests
    {
        private ProcessingData data;

        [SetUp]
        public void Init()
        {
            data = new ProcessingData();
            List<SingleProcessingData> list = new List<SingleProcessingData>();
            for (int i = 0; i < 10; i++)
            {
                list.Add(new SingleProcessingData { Text = i.ToString(), Stars = 5 });
            }

            data.Positive = list.ToArray();
            list.Clear();
            for (int i = 0; i < 10; i++)
            {
                list.Add(new SingleProcessingData { Text = i.ToString(), Stars = 1 });
            }

            data.Negative = list.ToArray();
            list.Clear();
            for (int i = 0; i < 10; i++)
            {
                list.Add(new SingleProcessingData { Text = i.ToString(), Stars = 3 });
            }

            data.Neutral = list.ToArray();
        }

        [Test]
        public void GetCrossValidation()
        {
            var validation = data.GetCrossValidation(0);

            Assert.AreEqual(3, validation.Testing.AllReviews.Count());

            Assert.AreEqual(1, validation.Testing.Negative.Count());
            Assert.AreEqual("9", validation.Testing.Negative.First().Text);
            Assert.AreEqual(1, validation.Testing.Negative.First().Stars);
            Assert.AreEqual(9, validation.Training.Negative.Count());
            Assert.AreEqual("8", validation.Training.Negative.ToArray()[8].Text);
            Assert.AreEqual(1, validation.Training.Negative.ToArray()[8].Stars);

            Assert.AreEqual(1, validation.Testing.Positive.Count());
            Assert.AreEqual("9", validation.Testing.Positive.First().Text);
            Assert.AreEqual(5, validation.Testing.Positive.First().Stars);
            Assert.AreEqual(9, validation.Training.Positive.Count());
            Assert.AreEqual("8", validation.Training.Positive.ToArray()[8].Text);
            Assert.AreEqual(5, validation.Training.Positive.ToArray()[8].Stars);

            Assert.AreEqual(1, validation.Testing.Neutral.Count());
            Assert.AreEqual("9", validation.Testing.Neutral.First().Text);
            Assert.AreEqual(3, validation.Testing.Neutral.First().Stars);
            Assert.AreEqual(9, validation.Training.Neutral.Count());
            Assert.AreEqual("8", validation.Training.Neutral.ToArray()[8].Text);
            Assert.AreEqual(3, validation.Training.Neutral.ToArray()[8].Stars);
        }


        [Test]
        public void AllReviews()
        {
            Assert.AreEqual(30, data.AllReviews.Count());
            Assert.AreEqual(10, data.Negative.Length);
            Assert.AreEqual(10, data.Positive.Length);
            Assert.AreEqual(10, data.Neutral.Length);
        }

        [Test]
        public void Fold()
        {
            var validation = data.GetCrossValidation(0, 5);

            Assert.AreEqual(2, validation.Testing.Negative.Count());
            Assert.AreEqual("8", validation.Testing.Negative.First().Text);
            Assert.AreEqual(8, validation.Training.Negative.Count());
            Assert.AreEqual("7", validation.Training.Negative.ToArray()[7].Text);

            Assert.AreEqual(2, validation.Testing.Positive.Count());
            Assert.AreEqual("8", validation.Testing.Positive.First().Text);
            Assert.AreEqual(8, validation.Training.Positive.Count());
            Assert.AreEqual("7", validation.Training.Positive.ToArray()[7].Text);
        }

        [Test]
        public void MoveData()
        {
            for (int i = 0; i < 10; i++)
            {
                Verify(i);
            }
        }

        private void Verify(int start)
        {
            var validation = data.GetCrossValidation(start);
            int verification = (9 + start) % 10;
            Assert.AreEqual(verification.ToString(), validation.Testing.Negative.First().Text);
            Assert.AreEqual(1, validation.Testing.Negative.First().Stars);
            Assert.AreEqual(verification.ToString(), validation.Testing.Positive.First().Text);
            Assert.AreEqual(5, validation.Testing.Positive.First().Stars);
            var negative = validation.Training.Negative.ToDictionary(item => item.Text, item => item);
            Assert.IsFalse(negative.ContainsKey(verification.ToString()));
        }
    }
}
