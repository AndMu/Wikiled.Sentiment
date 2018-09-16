using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.Tests.Aspects
{
    [TestFixture]
    public class MainAspectHandlerTests
    {
        private MainAspectHandler instance;

        [SetUp]
        public void Setup()
        {
            instance = new MainAspectHandler(new AspectContextFactory(), 0);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new MainAspectHandler(null));
            Assert.AreEqual(0, instance.GetFeatures(10).ToArray().Length);
            Assert.AreEqual(0, instance.GetAttributes(10).ToArray().Length);
        }

        [TestCase("I like my school teacher and teachers.", 0, 2, "teacher")]
        [TestCase("If ever enjoy professional basketball, with nike shoes, that will be a miracle", 1, 3, "shoes")]
        public async Task Process(string sentence, int totalAttributes, int totalFeatures, string topAttribute)
        {
            Assert.Throws<ArgumentNullException>(() => instance.Process(null));
            var data = await ActualWordsHandler.InstanceSimple.TextSplitter.Process(new ParseRequest(sentence)).ConfigureAwait(false);
            var review = ActualWordsHandler.InstanceSimple.Container.Resolve(data).Create();
            instance.Process(review);
            var attributes = instance.GetAttributes(10).ToArray();
            var features = instance.GetFeatures(10).ToArray();
            Assert.AreEqual(totalAttributes, attributes.Length);
            Assert.AreEqual(totalFeatures, features.Length);
            Assert.AreEqual(topAttribute, features.OrderByDescending(item => item.Text).First().Text);
        }
    }
}
