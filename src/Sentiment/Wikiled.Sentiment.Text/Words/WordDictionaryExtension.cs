using System.Collections.Generic;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Collection;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Words
{
    public static class WordDictionaryExtension
    {
        public static bool TryGetWordValue<T>(this IDictionary<string, T> table, IWordItem word, out T value)
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

            if (word.Entity == NamedEntities.Hashtag &&
                word.Text.Length > 1 &&
                table.TryGetValue(word.Text.Substring(1), out value))
            {
                return true;
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
