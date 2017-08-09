using System.Collections.Generic;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Tokenizer
{
    public class InvertorPipeline : IPipeline<WordEx>
    {
        public static readonly InvertorPipeline Instance = new InvertorPipeline();

        private InvertorPipeline() { }

        public IEnumerable<WordEx> Process(IEnumerable<WordEx> words)
        {
           int total = 0;
           bool invertor = false;
            foreach (var word in words)
            {
                total++;
                IWordItem item = word.UnderlyingWord as IWordItem;
                if (total >= 5 ||
                    (item != null && item.IsConjunction()))
                {
                    total = 0;
                    invertor = false;
                }

                if (item != null && 
                    item.IsInvertor)
                {
                    total = 0;
                    invertor = true;
                    continue;
                }

                if (invertor)
                {
                    var newResult = WordExFactory.Construct(item);
                    newResult.Text = "not_" + newResult.Text;
                    yield return newResult;
                }
                else
                {
                    yield return word;    
                }
            }
        }
    }
}
