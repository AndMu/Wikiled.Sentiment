using System.Collections.Generic;

namespace Wikiled.Sentiment.Text.Tokenizer
{
    public class LowerCasePipeline : IPipeline<string>
    {
        public static readonly LowerCasePipeline Instance = new LowerCasePipeline();

        private LowerCasePipeline()
        {
        }

        public IEnumerable<string> Process(IEnumerable<string> words)
        {
            foreach (var word in words)
            {
                if (!string.IsNullOrEmpty(word))
                {
                    yield return word.ToLower();
                }
            }
        }
    }
}
