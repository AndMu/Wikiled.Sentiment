using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Sentiment.Text.NLP.Style.Description.Data;
using Wikiled.Text.Analysis.NLP.Frequency;
using Wikiled.Text.Analysis.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Style.Obscurity
{
    public class SpecificObscrunity : IDataSource
    {
        private readonly ObscrunityData data = new ObscrunityData();

        private readonly IWordFrequencyList list;

        private Dictionary<string, int> wordsIndexes;

        public SpecificObscrunity(TextBlock text, IWordFrequencyList list)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            this.list = list ?? throw new ArgumentNullException(nameof(list));
        }

        public TextBlock Text { get; }

        [InfoField("Percentage in Top 100000 Words")]
        public double Top100000Words => data.Top100000Words;

        [InfoField("Percentage in Top 10000 Words")]
        public double Top10000Words => data.Top10000Words;

        [InfoField("Percentage in Top 1000 Words")]
        public double Top1000Words => data.Top1000Words;

        [InfoField("Percentage in Top 100 Words")]
        public double Top100Words => data.Top100Words;

        [InfoField("Percentage in Top 200000 Words")]
        public double Top200000Words => data.Top200000Words;

        [InfoField("Percentage in Top 300000 Words")]
        public double Top300000Words => data.Top300000Words;

        [InfoField("Percentage in Top 50000 Words")]
        public double Top50000Words => data.Top50000Words;

        [InfoField("Percentage in Top 5000 Words")]
        public double Top5000Words => data.Top5000Words;

        [InfoField("Percentage in Top 500 Words")]
        public double Top500Words => data.Top500Words;

        private Dictionary<string, int> WordsIndexes
        {
            get
            {
                if (wordsIndexes != null)
                {
                    return wordsIndexes;
                }

                wordsIndexes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                foreach (var word in Text.Words)
                {
                    var item = list.GetIndex(word.Text);
                    if (item != null)
                    {
                        wordsIndexes[word.Text] = list.GetIndex(word.Text).Index;
                    }
                }

                return wordsIndexes;
            }
        }

        public ObscrunityData GetData()
        {
            return (ObscrunityData)data.Clone();
        }

        public void Load()
        {
            data.Top100Words = CountPercentage(100);
            data.Top500Words = CountPercentage(500);
            data.Top1000Words = CountPercentage(1000);
            data.Top5000Words = CountPercentage(5000);
            data.Top10000Words = CountPercentage(10000);
            data.Top50000Words = CountPercentage(50000);
            data.Top100000Words = CountPercentage(100000);
            data.Top200000Words = CountPercentage(200000);
            data.Top300000Words = CountPercentage(300000);
        }

        private double CountPercentage(int top)
        {
            return WordsIndexes.Count(item => item.Value <= top) / (double)wordsIndexes.Count;
        }
    }
}
