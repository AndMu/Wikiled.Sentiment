using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Wikiled.Common.Logging;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Context
{
    public class WordsContext
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<WordsContext>();

        public WordsContext(WordEx word)
        {
            Word = word ?? throw new ArgumentNullException(nameof(word));
            Words = new List<WordEx>();
        }

        public double SentimentValue { get; set; }

        public WordEx Word { get; }

        public IList<WordEx> Words { get; }

        public void AddContext(WordEx word)
        {
            if (word == null)
            {
                throw new ArgumentNullException(nameof(word));
            }

            if (string.Compare(word.Text, Word.Text, StringComparison.OrdinalIgnoreCase) == 0)
            {
                log.LogDebug("This is owner word: {0}", word.Text);
                return;
            }

            Words.Add(word);
        }
    }
}
