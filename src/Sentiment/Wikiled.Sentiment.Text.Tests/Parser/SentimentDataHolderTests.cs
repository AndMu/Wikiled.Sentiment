﻿using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Text.Analysis.Dictionary.Streams;

namespace Wikiled.Sentiment.Text.Tests.Parser
{
    [TestFixture]
    public class SentimentDataHolderTests
    {
        private SentimentDataHolder sentimentData;

        [SetUp]
        public void Setup()
        {
            var path = ActualWordsHandler.InstanceSimple.Configuration.GetConfiguration("Resources");
            path = Path.Combine(path, @"Library\Standard");
            var stream = new DictionaryStream(Path.Combine(path, "EmotionLookupTable.txt"), new FileStreamSource());
            var data = stream.ReadDataFromStream(double.Parse).ToDictionary(item => item.Word, item => item.Value, StringComparer.OrdinalIgnoreCase);
            sentimentData = new SentimentDataHolder();
            sentimentData.PopulateEmotionsData(data);
        }

        [TestCase("good", "NN", 2)]
        [TestCase("bad", "NN", -2)]
        [TestCase("director", "NN", 0)]
        [TestCase("unfulfilled", "NN", -1)]
        [TestCase("EMOTICON_Joy", "NN", 2)]
        public void MeasureSentiment(string word, string pos, int sentiment)
        {
            var wordItem = ActualWordsHandler.InstanceSimple.WordFactory.CreateWord(word, pos);
            var measurment = sentimentData.MeasureSentiment(wordItem);
            Assert.AreEqual(sentiment, measurment?.DataValue.Value ?? 0);
        }

        [TestCase("good", 0.5, 0.5)]
        [TestCase("bad", 0.5, 0.5)]
        [TestCase("good", -0.5, -0.5)]
        [TestCase("bad", -0.5, -0.5)]
        public void Adjust(string word, double weight, double sentiment)
        {
            var wordItem = ActualWordsHandler.InstanceSimple.WordFactory.CreateWord(word, "NN");
            sentimentData.SetValue(word, new SentimentValueData(weight));
            var measurement = sentimentData.MeasureSentiment(wordItem);
            Assert.AreEqual(sentiment, measurement.DataValue.Value);
        }
    }
}
