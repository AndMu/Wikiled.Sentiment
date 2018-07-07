using System;
using System.Collections.Generic;
using Wikiled.Sentiment.Text.Data;

namespace Wikiled.Sentiment.Text.Words
{
    public static class NGramManager
    {
        public static NGramBlock[] GetNearNGram(this IWordItem[] words, int index, int length)
        {
            if (words is null)
            {
                throw new ArgumentNullException(nameof(words));
            }

            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            if (index <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (words.Length == 0)
            {
                throw new ArgumentException("Value cannot be an empty collection.", nameof(words));
            }

            int startingIndex = index - (length - 1);
            int endIndex = index + (length - 1);
            startingIndex = startingIndex < 0 ? 0 : startingIndex;
            endIndex = endIndex < words.Length ? endIndex : words.Length - 1;
            IWordItem[] subset = new IWordItem[endIndex - startingIndex + 1];
            for (int i = startingIndex; i <= endIndex; i++)
            {
                subset[i - startingIndex] = words[i];
            }

            return GetNGram(subset, length);
        }

        public static NGramBlock[] GetNGram(this IWordItem[] words, int length = 3)
        {
            if (words.Length < length)
            {
                return new NGramBlock[] { };
            }

            var result = new List<NGramBlock>();
            var wordOccurrences = new Queue<IWordItem>();

            for (int i = 0; i < words.Length; i++)
            {
                wordOccurrences.Enqueue(words[i]);
                if (wordOccurrences.Count == length)
                {
                    result.Add(new NGramBlock(wordOccurrences.ToArray()));
                    wordOccurrences.Dequeue();
                }
            }

            return result.ToArray();
        }
    }
}