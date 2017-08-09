using System.Linq;
using NUnit.Framework;
using Wikiled.Sentiment.Text.Tokenizer;

namespace Wikiled.Sentiment.Text.Tests.Tokenizer
{
    [TestFixture]
    public class CombinedSimpleWordPipelineTests
    {
        [Test]
        public void Create()
        {
            CombinedPipeline<string> combined = new CombinedPipeline<string>(LowerCasePipeline.Instance);
            Assert.AreEqual(1, combined.Pipelines.Count);
            combined = new CombinedPipeline<string>();
            Assert.AreEqual(0, combined.Pipelines.Count);
        }

        [Test]
        public void Process()
        {
            string[] data = new[] { "Test", string.Empty };
            CombinedPipeline<string> combined = new CombinedPipeline<string>(LowerCasePipeline.Instance);
            string[] results = combined.Process(data).ToArray();
            Assert.AreEqual(1, results.Length);
            Assert.AreEqual("test", results[0]);
        }
    }
}
