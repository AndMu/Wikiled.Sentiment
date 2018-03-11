using System.Collections.Generic;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Helpers;

namespace Wikiled.Sentiment.Text.Words
{
    public static class WordDictionaryExtension
    {
        public static bool TryGetWordValue<T>(this IDictionary<string, T> table, IWordItem word, out T value)
        {
            Guard.NotNull(() => table, table);
            Guard.NotNull(() => word, word);
            value = default(T);
            foreach (var text in word.GetPossibleText())
            {
                if (table.TryGetValue(text, out value))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetWordValue<T>(this MaskDictionary<T> table, IWordItem word, out T value)
        {
            Guard.NotNull(() => table, table);
            Guard.NotNull(() => word, word);
            if (table.TryGetValue(word.Text, out value))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(word.Stemmed) &&
                word.Stemmed != word.Text)
            {
                if (table.TryGetValue(word.Stemmed, out value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
