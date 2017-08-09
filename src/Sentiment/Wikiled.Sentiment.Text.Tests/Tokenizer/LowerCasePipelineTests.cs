using System.Linq;
using NUnit.Framework;
using Wikiled.Sentiment.Text.Tokenizer;

namespace Wikiled.Sentiment.Text.Tests.Tokenizer
{
    [TestFixture]
    public class LowerCasePipelineTests
    {
        [Test]
        public void Process()
        {
            string[] data = new[] {"Test", string.Empty};
            string[] results = LowerCasePipeline.Instance.Process(data).ToArray();
            Assert.AreEqual(1, results.Length);
            Assert.AreEqual("test", results[0]);
        }
    }
}
