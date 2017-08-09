using System.Collections.Generic;

namespace Wikiled.Sentiment.Text.Tokenizer
{
    public class PunctuationPipeline : IPipeline<string>
    {
        public static readonly PunctuationPipeline Instance = new PunctuationPipeline();

        readonly Dictionary<char, bool> nonPunctuations = new Dictionary<char, bool>();
        private PunctuationPipeline()
        {
            nonPunctuations['$'] = true;
            nonPunctuations['£'] = true;
            nonPunctuations['%'] = true;
            nonPunctuations['^'] = true;
            nonPunctuations['&'] = true;
            nonPunctuations['_'] = true;
            nonPunctuations['#'] = true;
        }

        public IEnumerable<string> Process(IEnumerable<string> words)
        {
            Stack<string> last = new Stack<string>(2);
            foreach (var word in words)
            {
                string currentWord = word;

                if (currentWord.Length == 1)
                {
                    yield return currentWord;
                    continue;
                }
                while (currentWord.Length > 1 &&
                       IsPunctuation(currentWord[0]))
                {
                    yield return currentWord[0].ToString();
                    currentWord = currentWord.Substring(1, currentWord.Length - 1);
                }

                if (currentWord.Length == 1)
                {
                    yield return currentWord;
                    continue;
                }
                while (currentWord.Length > 1 &&
                       IsPunctuation(currentWord[currentWord.Length - 1]))
                {
                    last.Push(currentWord[currentWord.Length - 1].ToString());
                    currentWord = currentWord.Substring(0, currentWord.Length - 1);
                }

                if (currentWord.Length == 1)
                {
                    yield return currentWord;
                    continue;
                }

                yield return currentWord;
                int lastWords = last.Count;
                for (int i = 0; i < lastWords; i++)
                {
                    yield return last.Pop();
                }
            }
        }

        public bool IsPunctuation(char letter)
        {
            return char.IsPunctuation(letter) && !nonPunctuations.ContainsKey(letter);
        }
    }
}
