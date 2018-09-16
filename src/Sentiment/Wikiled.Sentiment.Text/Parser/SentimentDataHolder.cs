using System;
using System.Collections.Generic;
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

        public static SentimentDataHolder PopulateEmotionsData(Dictionary<string, double> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            SentimentDataHolder instance = new SentimentDataHolder();
            foreach (var item in data)
            {
                var value = new SentimentValueData(item.Value);
                if (item.Key[item.Key.Length - 1] == '*')
                {
                    string word = item.Key.Substring(0, item.Key.Length - 1);
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

            foreach (var emoji in EmojiSentiment.Positive)
            {
                instance.SetValue(emoji.AsShortcode(), new SentimentValueData(2));
            }

            foreach (var emoji in EmojiSentiment.Negative)
            {
                instance.SetValue(emoji.AsShortcode(), new SentimentValueData(-2));
            }

            return instance;
        }

        private void SetValue(string word, SentimentValueData value)
        {
            EmotionsTable.Remove(word);
            AddSentimentValue(word, value);
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
