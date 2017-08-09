using System.Collections.Generic;

namespace Wikiled.Sentiment.Text.Tokenizer
{
    public class WordCleanupPipeline : IPipeline<string>
    {
        public static readonly WordCleanupPipeline Instance = new WordCleanupPipeline();

        private WordCleanupPipeline()
        {
        }

        public IEnumerable<string> Process(IEnumerable<string> words)
        {
            foreach (var word in words)
            {
                string currentWord = word;
                if (string.IsNullOrEmpty(currentWord))
                {
                    continue;
                }

                currentWord = currentWord.TrimEnd('\r', '\n');
                yield return currentWord;
            }
        }
    }
}
