using System;
using System.Linq;
using Wikiled.Sentiment.Text.Helpers;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.MachineLearning.Statistics
{
    public class WordDistributionCalculator : BaseDistributionCalculator
    {
        public WordDistributionCalculator(WordEx word, params Document[] documents)
            : base(documents)
        {
            MainItem = word ?? throw new ArgumentNullException(nameof(word));
        }

        public WordEx MainItem { get; }

        protected override double? GetValue()
        {
            return MainItem.Value;
        }

        protected override void Process(Document currentDocument, StatisticsResult result)
        {
            AutoEvictingDictionary<WordEx, IStatisticsResult> autoEvictingDictionary =
                new AutoEvictingDictionary<WordEx, IStatisticsResult>(length: result.WindowsSize);
            var words = currentDocument.Words.ToArray();
            for (int i = 0; i < words.Length; i++)
            {
                autoEvictingDictionary.Increment();
                WordEx currentWord = words[i];
                IStatisticsResult statistics = IsOurWord(words[i])
                                                   ? (IStatisticsResult)result
                                                   : new NullStatisticsResult(words[i].Value);

                foreach (var statisticsResult in autoEvictingDictionary.Values)
                {
                    statisticsResult.AddData(currentWord.Value);
                    statistics.AddData(statisticsResult.Value);
                }

                autoEvictingDictionary.Add(words[i], statistics);
                result.IncrementTotal();
                statistics.IncrementOccurences();
            }
        }

        private bool IsOurWord(WordEx word)
        {
            if (string.Compare(MainItem.Text, word.Text, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            IWordItem mainUnderlying = MainItem.UnderlyingWord as IWordItem;
            if (word.UnderlyingWord is IWordItem underlying && mainUnderlying != null)
            {
                return SimpleWordItemEquality.Instance.Equals(underlying, mainUnderlying);
            }

            return false;
        }
    }
}
