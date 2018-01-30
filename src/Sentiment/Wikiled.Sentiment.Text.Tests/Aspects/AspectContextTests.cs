using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.Tests.Aspects
{
    [TestFixture]
    public class AspectContextTests
    {
        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new AspectContext(true, null));
        }

        [Test]
        public async Task Process()
        {
            var data = await ActualWordsHandler.Instance.TextSplitter.Process(new ParseRequest("I like my school teacher.")).ConfigureAwait(false);

            var review = new ParsedReviewManager(ActualWordsHandler.Instance.WordsHandler, data).Create();
            var context = new AspectContext(true, review.Items.ToArray());
            context.Process();
            var attributes = context.GetAttributes().ToArray();
            var features = context.GetFeatures().ToArray();
            Assert.AreEqual(1, attributes.Length);
            Assert.AreEqual("like", attributes[0].Text);
            Assert.AreEqual(2, features.Length);
            Assert.IsTrue(features.Any(item => item.Text == "school"));
            Assert.IsTrue(features.Any(item => item.Text == "teacher"));
        }
    }
}
