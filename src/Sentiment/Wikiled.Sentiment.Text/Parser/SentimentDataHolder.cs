using System;
using System.Collections.Generic;
using Wikiled.Sentiment.Text.Helpers;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Parser
{
    public class SentimentDataHolder : ISentimentDataHolder
    {
        private MaskDictionary<SentimentValueData> EmotionsLookup { get; } = new MaskDictionary<SentimentValueData>(StringComparer.OrdinalIgnoreCase);

        private Dictionary<string, SentimentValueData> EmotionsTable { get; } = new Dictionary<string, SentimentValueData>(StringComparer.OrdinalIgnoreCase);

        public SentimentValue MeasureSentiment(IWordItem word)
        {
            if (!EmotionsTable.TryGetWordValue(word, out var value))
            {
                return MeasureLookupSentiment(word);
            }

            var sentiment = new SentimentValue(word, new SentimentValueData(value.Value));
            return sentiment;
        }

        public static ISentimentDataHolder Load(string file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var reader = new SentimentDataReader(file);
            return Load(reader.Read());
        }

        public static ISentimentDataHolder Load(IEnumerable<WordSentimentValueData> reader)
        {
            var holder = new SentimentDataHolder();
            foreach (var valueData in reader)
            {
                holder.SetValue(valueData.Word, valueData.Data);
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
                if (item.Key[item.Key.Length - 1] == '*')
                {
                    var word = item.Key.Substring(0, item.Key.Length - 1);
                    instance.SetValue(word, value);
                    if (word.Length > 4)
                    {
                        instance.EmotionsLookup.Add(string.Intern(word), value);
                    }
                }
                else
                {
                    instance.SetValue(item.Key, value);
                }
            }

            return instance;
        }

        private void SetValue(string word, SentimentValueData value)
        {
            EmotionsTable.Remove(word);
            AddSentimentValue(word, value);
        }

        private void AddSentimentValue(string word, SentimentValueData value)
        {
            if (EmotionsTable.ContainsKey(word))
            {
                return;
            }

            EmotionsTable[string.Intern(word)] = value;
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
            var sentiment = new SentimentValue(word, new SentimentValueData(value.Value * invertingEnd));
            return sentiment;
        }
    }
}