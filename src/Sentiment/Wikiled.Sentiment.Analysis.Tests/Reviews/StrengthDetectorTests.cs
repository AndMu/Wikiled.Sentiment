using Wikiled.Sentiment.Analysis.Review;
using Wikiled.Sentiment.Text.MachineLearning;
using NUnit.Framework;
using Rhino.Mocks;
using Wikiled.Arff.Normalization;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Analysis.Tests.Reviews
{
    [TestFixture]
    public class StrengthDetectorTests
    {
        private MockRepository mock;

        private IMachineSentiment negative;

        private IMachineSentiment positive;

        [SetUp]
        public void Setup()
        {
            mock = new MockRepository();
            positive = mock.StrictMock<IMachineSentiment>();
            negative = mock.StrictMock<IMachineSentiment>();
        }

        [Test]
        public void ResolvePositive()
        {
            VectorData data = new VectorData(new VectorCell[]{}, 0, NormalizationType.None);
            Expect.Call(negative.GetVector(null, NormalizationType.None))
                .IgnoreArguments()
                .Return(data);
            Expect.Call(negative.Predict(data))
                .Return(1);
            mock.ReplayAll();
            StrengthDetector detector = new StrengthDetector(positive, negative);
            var result = detector.Resolve(new RatingData
                                              {
                                                  Negative = 10,
                                                  Positive = 1
                                              });
            Assert.AreEqual(2, result);
            mock.VerifyAll();
        }

        [Test]
        public void ResolveNegative()
        {
            VectorData data = new VectorData(new VectorCell[] { }, 0, NormalizationType.None);
            Expect.Call(positive.GetVector(null, NormalizationType.None))
                .IgnoreArguments()
                .Return(data);
            Expect.Call(positive.Predict(data))
                .Return(2);
            mock.ReplayAll();
            StrengthDetector detector = new StrengthDetector(positive, negative);
            var result = detector.Resolve(new RatingData
            {
                Negative = 1,
                Positive = 10
            });
            Assert.AreEqual(5, result);
            mock.VerifyAll();
        }
    }
}
