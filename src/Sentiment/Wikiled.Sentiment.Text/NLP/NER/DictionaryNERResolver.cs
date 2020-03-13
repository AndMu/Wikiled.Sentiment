using System;
using System.Collections.Generic;
using SharpNL.Utility;

namespace Wikiled.Sentiment.Text.NLP.NER
{
    public class DictionaryNERResolver : INamedEntityRecognition
    {
        private readonly Dictionary<string, string> nerTable;

        public DictionaryNERResolver(Dictionary<string, string> nerTable)
        {
            this.nerTable = nerTable ?? throw new ArgumentNullException(nameof(nerTable));
        }

        public IEnumerable<Span> Tag(string[] words)
        {
            for (int i = 0; i < words.Length; i++)
            {
                if (nerTable.TryGetValue(words[i], out var type))
                {
                    yield return new Span(i, i + 1, type);
                }
            }
        }
    }
}
