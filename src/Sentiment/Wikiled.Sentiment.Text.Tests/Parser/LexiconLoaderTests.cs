using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Wikiled.Common.Utilities.Resources.Config;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Config;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.Tests.Parser
{
    [TestFixture]
    public class LexiconLoaderTests
    {
        private LexiconLoader instance;

        private LexiconConfig config;

        [SetUp]
        public void SetUp()
        {
            config = new LexiconConfig();
            config.Lexicons = new LocationConfig();
            config.Resources = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data");
            config.Lexicons.Local = "Lexicons";
            instance = CreateInstance();
        }

        [Test]
        public void Load()
        {
            
            instance.Load();
            Assert.AreEqual(2, instance.Supported.Count());
            var lexicons = instance.Supported.OrderBy(item => item).ToArray();
            Assert.AreEqual("base", lexicons[0]);
            Assert.AreEqual("other", lexicons[1]);
        }

        [Test]
        public void GetLexicon()
        {
            instance.Load();
            Assert.Throws<ArgumentOutOfRangeException>(() => instance.GetLexicon("Unknown"));
            var word = new TestWordItem("one");
            var result = instance.GetLexicon("base").MeasureSentiment(word).DataValue.Value;
            Assert.AreEqual(1, result);

            result = instance.GetLexicon("other").MeasureSentiment(word).DataValue.Value;
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void CheckArguments()
        {
            Assert.Throws<ArgumentException>(() => instance.GetLexicon(null));
            Assert.Throws<ArgumentNullException>(() =>
            {
                var data = instance.Supported;
            });
        }

        private LexiconLoader CreateInstance()
        {
            return new LexiconLoader(new NullLogger<LexiconLoader>(), config);
        }
    }
}
