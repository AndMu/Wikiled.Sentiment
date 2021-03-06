﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.Text.Helpers;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Parser
{
    public class SentimentDataHolder : ISentimentDataHolder
    {
        private MaskDictionary<SentimentValueData> EmotionsLookup { get; } = new MaskDictionary<SentimentValueData>(StringComparer.OrdinalIgnoreCase);

        private Dictionary<string, WordSentimentValueData> EmotionsTable { get; } = new Dictionary<string, WordSentimentValueData>(StringComparer.OrdinalIgnoreCase);

        private readonly Lazy<double> averageStrength;

        private static readonly ILogger logger = ApplicationLogging.LoggerFactory.CreateLogger<SentimentDataHolder>();

        public SentimentDataHolder()
        {
            averageStrength = new Lazy<double>(() => EmotionsLookup.All.Select(item => item.Value).Concat(EmotionsTable.Values.Select(item => item.Data.Value)).Average(Math.Abs));
        }

        public IEnumerable<WordSentimentValueData> Values => EmotionsTable.Values;

        public SentimentValue MeasureSentiment(IWordItem word)
        {
            if (!EmotionsTable.TryGetWordValue(word, out var value))
            {
                return MeasureLookupSentiment(word);
            }

            var sentiment = new SentimentValue(word, value.Word, new SentimentValueData(value.Data.Value));
            return sentiment;
        }

        public double AverageStrength => averageStrength.Value;

        public void Merge(ISentimentDataHolder holder)
        {
            foreach (var value in holder.Values)
            {
                SetValue(value);
            }
        }

        public static ISentimentDataHolder Load(string file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            logger.LogDebug("Load {0}", file);
            var reader = new SentimentDataReader(file);
            return Load(reader.Read());
        }

        public static ISentimentDataHolder Load(IEnumerable<WordSentimentValueData> reader)
        {
            var holder = new SentimentDataHolder();
            foreach (var valueData in reader)
            {
                if (!SetMasked(valueData.Word, holder, valueData.Data))
                {
                    holder.SetValue(valueData);
                }
            }

            return holder;
        }

        public static SentimentDataHolder PopulateEmotionsData(Dictionary<string, double> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var instance = new SentimentDataHolder();
            foreach (var item in data)
            {
                var value = new SentimentValueData(item.Value);
                if (!SetMasked(item.Key, instance, value))
                {
                    instance.SetValue(new WordSentimentValueData(item.Key, value));
                }
            }

            return instance;
        }

        private static bool SetMasked(string wordMask, SentimentDataHolder instance, SentimentValueData value)
        {
            if (wordMask[wordMask.Length - 1] != '*')
            {
                return false;
            }

            var word = string.Intern(wordMask.Substring(0, wordMask.Length - 1));
            instance.SetValue(new WordSentimentValueData(word, value));
            if (word.Length > 4)
            {
                instance.EmotionsLookup.Add(word, value);
            }
            else
            {
                logger.LogWarning("Ignoring masked {0} as it is too short", word);
            }

            return true;

        }

        private void SetValue(WordSentimentValueData data)
        {
            EmotionsTable[string.Intern(data.Word)] = data;
        }

        private SentimentValue MeasureLookupSentiment(IWordItem word)
        {
            if (word is IPhrase ||
                !EmotionsLookup.TryGetWordValue(word, out var value))
            {
                return null;
            }

            var isInverted = word.Text.EndsWith("free", StringComparison.OrdinalIgnoreCase) ||
                word.Text.EndsWith("proof", StringComparison.OrdinalIgnoreCase) ||
                word.Text.EndsWith("less", StringComparison.OrdinalIgnoreCase);
            var invertingEnd = isInverted ? -1 : 1;
            var sentiment = new SentimentValue(word, word.Text, new SentimentValueData(value.Value * invertingEnd));
            return sentiment;
        }
    }
}