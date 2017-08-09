using System.Collections.Generic;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Tokenizer
{
    public class NullWordItemPipeline : IPipeline<WordEx>
    {
        public static readonly NullWordItemPipeline Instance = new NullWordItemPipeline();

        private NullWordItemPipeline(){}

        public IEnumerable<WordEx> Process(IEnumerable<WordEx> words)
        {
            return words;
        }
    }
}
