using System;
using System.Collections.Generic;
using NLog;
using Wikiled.Common.Arguments;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Processing.Context
{
    public class WordsContext
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public WordsContext(WordEx word)
        {
            Guard.NotNull(() => word, word);
            Word = word;
            Words = new List<WordEx>();
        }

        public double SentimentValue { get; set; }

        public WordEx Word { get; }

        public IList<WordEx> Words { get; }

        public void AddContext(WordEx word)
        {
            Guard.NotNull(() => word, word);
            if (string.Compare(word.Text, Word.Text, StringComparison.OrdinalIgnoreCase) == 0)
            {
                log.Debug("This is owner word: {0}", word.Text);
                return;
            }

            Words.Add(word);
        }
    }
}
