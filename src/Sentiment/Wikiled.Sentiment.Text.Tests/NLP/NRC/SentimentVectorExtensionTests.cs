using NUnit.Framework;
using Wikiled.Arff.Normalization;
using Wikiled.Sentiment.Text.NLP.NRC;

namespace Wikiled.Sentiment.Text.Tests.NLP.NRC
{
    [TestFixture]
    public class SentimentVectorExtensionTests
    {
        [Test]
        public void GetVector()
        {
            SentimentVector vector = new SentimentVector();
            var vectorData = vector.GetVector(NormalizationType.None);
            Assert.AreEqual("Anger", vectorData.Cells[0].Data.Name);
        }
    }
}
