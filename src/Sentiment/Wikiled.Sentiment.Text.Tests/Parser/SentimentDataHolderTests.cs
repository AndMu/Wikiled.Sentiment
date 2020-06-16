using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Config;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Text.Analysis.Dictionary.Streams;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Utilities.Resources;
using Wikiled.Common.Utilities.Resources.Config;

namespace Wikiled.Sentiment.Text.Tests.Parser
{
    [TestFixture]
    public class SentimentDataHolderTests
    {
        private SentimentDataHolder sentimentData;

        [SetUp]
        public void Setup()
        {
            var loader = new LexiconConfigLoader(ApplicationLogging.LoggerFactory.CreateLogger<LexiconConfigLoader>());
            var config = loader.Load(TestContext.CurrentContext.TestDirectory);
            var path = config.GetFullPath(item => item.Model);
            var stream = new DictionaryStream(Path.Combine(path, "EmotionLookupTable.txt"), new FileStreamSource());
            var data = stream.ReadDataFromStream(double.Parse).ToDictionary(item => item.Word, item => item.Value, StringComparer.OrdinalIgnoreCase);
            sentimentData = SentimentDataHolder.PopulateEmotionsData(data);
        }

        [Test]
        public void Average()
        {
            var value = sentimentData.AverageStrength;
            Assert.AreEqual(1.71, Math.Round(value, 2));
        }

        [TestCase("good", "NN", 2)]
        [TestCase("bad", "NN", -2)]
        [TestCase("director", "NN", 0)]
        [TestCase("unfulfilled", "NN", -1)]
        [TestCase("EMOTICON_Joy", "NN", 2)]
        public void MeasureSentiment(string word, string pos, int sentiment)
        {
            Text.Words.IWordItem wordItem = ActualWordsHandler.InstanceSimple.WordFactory.CreateWord(word, pos);
            SentimentValue measurement = sentimentData.MeasureSentiment(wordItem);
            Assert.AreEqual(sentiment, measurement?.DataValue.Value ?? 0);
        }

        [TestCase("good", 0.5, 0.5)]
        [TestCase("bad", 0.5, 0.5)]
        [TestCase("good", -0.5, -0.5)]
        [TestCase("bad", -0.5, -0.5)]
        public void Adjust(string word, double weight, double sentiment)
        {
            var table = new Dictionary<string, double>();
            table[word] = weight;
            sentimentData = SentimentDataHolder.PopulateEmotionsData(table);
            Text.Words.IWordItem wordItem = ActualWordsHandler.InstanceSimple.WordFactory.CreateWord(word, "NN");
            SentimentValue measurement = sentimentData.MeasureSentiment(wordItem);
            Assert.AreEqual(sentiment, measurement.DataValue.Value);
        }
    }
}
