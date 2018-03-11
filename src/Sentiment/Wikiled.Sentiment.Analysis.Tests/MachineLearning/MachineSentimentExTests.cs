using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.Text.MachineLearning;

namespace Wikiled.Sentiment.Analysis.Tests.MachineLearning
{
    [TestFixture]
    public class MachineSentimentExTests
    {
        private Classifier instance;

        private string fileName;

        [SetUp]
        public void SetUp()
        {
            instance = CreateMachineSentimentEx();
            fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "test.svm");
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        [Test]
        public void Train()
        {
            // Example binary data
            double[][] inputs = { new double[] { -1, -1 }, new double[] { -1, 1 }, new double[] { 1, -1 }, new double[] { 1, 1 } };

            int[] xor = // xor labels
                {
                    -1, 1, 1, -1
                };
            instance.Train(xor, inputs, CancellationToken.None);
            var result = instance.Classify(inputs);
            Assert.AreEqual(4, result.Length);
            instance.Save(fileName);
            Assert.IsTrue(File.Exists(fileName));
            instance.Load(fileName);
        }

        private Classifier CreateMachineSentimentEx()
        {
            return new Classifier();
        }
    }
}