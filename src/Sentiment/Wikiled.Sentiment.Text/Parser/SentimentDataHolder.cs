using System;
using System.Collections.Generic;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Helpers;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Twitter;

namespace Wikiled.Sentiment.Text.Parser
{
    public class SentimentDataHolder : ISentimentDataHolder
    {
        private MaskDictionary<SentimentValueData> EmotionsLookup { get; } = new MaskDictionary<SentimentValueData>(StringComparer.OrdinalIgnoreCase);

        private Dictionary<string, SentimentValueData> EmotionsTable { get; } = new Dictionary<string, SentimentValueData>(StringComparer.OrdinalIgnoreCase);

        public static ISentimentDataHolder Load(IEnumerable<WordSentimentValueData> reader)
        {
            SentimentDataHolder holder = new SentimentDataHolder();
            foreach (var valueData in reader)
            {
                holder.SetValue(valueData.Word, valueData.Data);
            }

            return holder;
        }

        public void SetValue(string word, SentimentValueData value)
        {
            EmotionsTable.Remove(word);
            AddSentimentValue(word, value);
        }

        public void Clear()
        {
            EmotionsLookup.Clear();
            EmotionsTable.Clear();
        }

        public Dictionary<string, SentimentValueData> CreateEmotionsData()
        {
            Dictionary<string, SentimentValueData> table = new Dictionary<string, SentimentValueData>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in EmotionsTable)
            {
                table[string.Intern(item.Key)] = item.Value;
            }

            foreach (var item in EmotionsLookup)
            {
                table[string.Intern(item.Key + "*")] = item.Value;
            }

            return table;
        }

        public SentimentValue MeasureSentiment(IWordItem word)
        {
            if (!EmotionsTable.TryGetWordValue(word, out SentimentValueData value))
            {
                return MeasureLookupSentiment(word);
            }

            var sentiment = new SentimentValue(word, new SentimentValueData(value.Value));
            return sentiment;
        }

        public void PopulateEmotionsData(Dictionary<string, double> data)
        {
            Guard.NotNull(() => data, data);
            Clear();
            foreach (var item in data)
            {
                var value = new SentimentValueData(item.Value);
                if (item.Key[item.Key.Length - 1] == '*')
                {
                    string word = item.Key.Substring(0, item.Key.Length - 1);
                    SetValue(word, value);
                    if (word.Length > 4)
                    {
                        EmotionsLookup.Add(string.Intern(word), value);
                    }
                }
                else
                {
                    SetValue(item.Key, value);
                }
            }

            foreach (var emoji in EmojiSentiment.Positive)
            {
                SetValue(emoji.AsShortcode(), new SentimentValueData(2));
            }

            foreach (var emoji in EmojiSentiment.Negative)
            {
                SetValue(emoji.AsShortcode(), new SentimentValueData(-2));
            }
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
                !EmotionsLookup.TryGetWordValue(word, out SentimentValueData value))
            {
                return null;
            }

            bool isInverted = word.Text.EndsWith("free", StringComparison.OrdinalIgnoreCase) ||
                              word.Text.EndsWith("proof", StringComparison.OrdinalIgnoreCase) ||
                              word.Text.EndsWith("less", StringComparison.OrdinalIgnoreCase);
            var invertingEnd = isInverted ? -1 : 1;
            var sentiment = new SentimentValue(word, new SentimentValueData(value.Value * invertingEnd));
            return sentiment;
        }
    }
}
