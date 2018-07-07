using System;
using System.Collections.Generic;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Tokenizer
{
    public class WordItemFilterOutPipeline : IPipeline<WordEx>
    {
        private readonly Func<IWordItem, bool> condition;

        public WordItemFilterOutPipeline(Func<IWordItem, bool> condition)
        {
            this.condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }

        public IEnumerable<WordEx> Process(IEnumerable<WordEx> words)
        {
            foreach (var word in words)
            {
                if (word.UnderlyingWord is IWordItem wordItem &&
                    condition(wordItem))
                {
                    continue;
                }

                yield return word;
            }
        }
    }
}
