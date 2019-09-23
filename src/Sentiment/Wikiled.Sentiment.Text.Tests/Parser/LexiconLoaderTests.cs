using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.Tests.Parser
{
    [TestFixture]
    public class LexiconLoaderTests
    {
        private LexiconLoader instance;

        [SetUp]
        public void SetUp()
        {
            instance = CreateInstance();
        }

        [Test]
        public void Load()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "Lexicons");
            instance.Load(path);
            Assert.AreEqual(2, instance.Supported.Count());
            var lexicons = instance.Supported.OrderBy(item => item).ToArray();
            Assert.AreEqual("base", lexicons[0]);
            Assert.AreEqual("other", lexicons[1]);
        }

        [Test]
        public void GetLexicon()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "Lexicons");
            instance.Load(path);
            Assert.Throws<ArgumentOutOfRangeException>(() => instance.GetLexicon("Unknown"));
            var word = new TestWordItem();
            word.Text = "one";
            var result = instance.GetLexicon("base").MeasureSentiment(word).DataValue.Value;
            Assert.AreEqual(1, result);

            result = instance.GetLexicon("other").MeasureSentiment(word).DataValue.Value;
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void CheckArguments()
        {
            Assert.Throws<ArgumentException>(() => instance.GetLexicon(null));
            Assert.Throws<ArgumentException>(() => instance.Load(null));
            Assert.Throws<ArgumentNullException>(() =>
            {
                var data = instance.Supported;
            });
        }

        private LexiconLoader CreateInstance()
        {
            return new LexiconLoader(new NullLogger<ContextWordsDataLoader>());
        }
    }
}
