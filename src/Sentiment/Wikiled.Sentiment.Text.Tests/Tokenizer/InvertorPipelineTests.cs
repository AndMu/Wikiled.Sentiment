using System.Linq;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Text.Analysis.POS.Tags;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Sentiment.Text.Tokenizer;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Tests.Tokenizer
{
    [TestFixture]
    public class InvertorPipelineTests
    {
        [Test]
        public void Process()
        {
            WordEx[] data =
            {
                WordExFactory.Construct(new TestWordItem {Text = "One"}),
                WordExFactory.Construct(new TestWordItem {Text = "Two", IsInvertor = true}),
                WordExFactory.Construct(new TestWordItem {Text = "Three"}),
                WordExFactory.Construct(new TestWordItem {Text = "Four"}),
                WordExFactory.Construct(new TestWordItem {Text = "X", POS = CoordinatingConjunction.Instance}),
                WordExFactory.Construct(new TestWordItem {Text = "Five"})
            };

            WordEx[] result = InvertorPipeline.Instance.Process(data).ToArray();
            Assert.AreEqual(5, result.Length);
            Assert.AreEqual("One", result[0].ItemText);
            Assert.AreEqual("Three", result[1].ItemText);
            Assert.AreEqual("not_Three", result[1].Text);
            Assert.AreEqual("Four", result[2].ItemText);
            Assert.AreEqual("not_Four", result[2].Text);
            Assert.AreEqual("X", result[3].ItemText);
            Assert.AreEqual("X", result[3].Text);
            Assert.AreEqual("Five", result[4].ItemText);
            Assert.AreEqual("Five", result[4].Text);
        }
    }
}
