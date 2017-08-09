using System.Collections.Generic;

namespace Wikiled.Sentiment.Text.Tokenizer
{
    public class NullSimpleWordPipeline : IPipeline<string>
    {
        public static readonly NullSimpleWordPipeline Instance = new NullSimpleWordPipeline();

        private NullSimpleWordPipeline(){}

        public IEnumerable<string> Process(IEnumerable<string> words)
        {
            return words;
        }
    }
}
