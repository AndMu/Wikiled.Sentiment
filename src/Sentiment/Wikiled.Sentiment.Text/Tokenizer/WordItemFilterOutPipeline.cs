using System;
using System.Collections.Generic;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Tokenizer
{
    public class WordItemFilterOutPipeline : IPipeline<WordEx>
    {
        private readonly Func<IWordItem, bool> condition;

        public WordItemFilterOutPipeline(Func<IWordItem, bool> condition)
        {
            Guard.NotNull(() => condition, condition);
            this.condition = condition;
        }

        public IEnumerable<WordEx> Process(IEnumerable<WordEx> words)
        {
            foreach (var word in words)
            {
                IWordItem wordItem = word.UnderlyingWord as IWordItem;
                if (wordItem != null &&
                    condition(wordItem))
                {
                    continue;
                }

                yield return word;
            }
        }
    }
}
