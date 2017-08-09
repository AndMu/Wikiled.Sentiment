using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.NLP.Style.Description.Data;
using Wikiled.Sentiment.Text.Reflection;
using Wikiled.Text.Analysis.NLP.Frequency;

namespace Wikiled.Sentiment.Text.NLP.Style.Obscurity
{
    public class SpecificObscrunity : IDataSource
    {
        private Dictionary<string, int> wordsIndexes;

        private readonly ObscrunityData data = new ObscrunityData();

        private readonly IWordFrequencyList list;

        public SpecificObscrunity(TextBlock text, IWordFrequencyList list)
        {
            Guard.NotNull(() => list, list);
            Text = text;
            this.list = list;
        }

        public uint MyProperty { get; set; }

        protected Dictionary<string, int> WordsIndexes
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
                    wordsIndexes[word.Text] = list.GetIndex(word.Text);
                }

                return wordsIndexes;
            }
        }

        public TextBlock Text { get; private set; }

        [InfoField("Percentage in Top 100 Words")]
        public double Top100Words
        {
            get
            {
                return data.Top100Words;
            }
        }

        [InfoField("Percentage in Top 500 Words")]
        public double Top500Words
        {
            get
            {
                return data.Top500Words;
            }
        }
        
        [InfoField("Percentage in Top 1000 Words")]
        public double Top1000Words
        {
            get
            {
                return data.Top1000Words;
            }
        }
        
        [InfoField("Percentage in Top 5000 Words")]
        public double Top5000Words
        {
            get
            {
                return data.Top5000Words;
            }
        }

        [InfoField("Percentage in Top 10000 Words")]
        public double Top10000Words
        {
            get
            {
                return data.Top10000Words;
            }
        }
        
        [InfoField("Percentage in Top 50000 Words")]
        public double Top50000Words
        {
            get
            {
                return data.Top50000Words;
            }
        }
        
        [InfoField("Percentage in Top 100000 Words")]
        public double Top100000Words
        {
            get
            {
                return data.Top100000Words;
            }
        }
        
        [InfoField("Percentage in Top 200000 Words")]
        public double Top200000Words
        {
            get
            {
                return data.Top200000Words;
            }
        }
        
        [InfoField("Percentage in Top 300000 Words")]
        public double Top300000Words
        {
            get
            {
                return data.Top300000Words;
            }
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

        public ObscrunityData GetData()
        {
            return (ObscrunityData)data.Clone();
        }

        private double CountPercentage(int top)
        {
            return WordsIndexes.Count(item => item.Value <= top) / (double)wordsIndexes.Count;
        }
    }
}
