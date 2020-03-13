using System;
using System.Collections.Generic;
using System.Linq;
using SharpNL.Utility;

namespace Wikiled.Sentiment.Text.NLP.NER
{
    public class DynamicNERResolver : INamedEntityRecognition
    {
        private readonly IEnumerable<Func<string, string>> nerLookup;

        public DynamicNERResolver(IEnumerable<Func<string, string>> nerLookup)
        {
            this.nerLookup = nerLookup ?? throw new ArgumentNullException(nameof(nerLookup));
        }

        public IEnumerable<Span> Tag(string[] words)
        {
            for (int i = 0; i < words.Length; i++)
            {
                var type = nerLookup.Select(item => item(words[i])).FirstOrDefault(item => item != null);
                if (type != null)
                {
                    yield return new Span(i, i + 1, type);
                }
            }
        }
    }
}
