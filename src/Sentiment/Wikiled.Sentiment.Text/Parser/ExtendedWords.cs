using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wikiled.Sentiment.Text.Config;
using Wikiled.Text.Analysis.Dictionary.Streams;

namespace Wikiled.Sentiment.Text.Parser
{
    public class ExtendedWords : IExtendedWords
    {
        private Dictionary<string, int> idiomsSentiment;

        private readonly Dictionary<string, string> replacements = new Dictionary<string, string>();

        private readonly string resourcesPath;

        public ExtendedWords(ILexiconConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            resourcesPath = Path.Combine(config.FullLexiconPath, "Repair");
            ReadIdioms();
            ReadSlang();
        }

        public IEnumerable<(string Word, int Sentiment)> GetSentiments()
        {
            return idiomsSentiment.Select(item => (item.Key, item.Value));
        }

        public IEnumerable<(string Word, string Replacement)> GetReplacements()
        {
            return replacements.Select(item => (item.Key, item.Value));
        }

        public bool HasAlternative(string word, out string alternative)
        {
            return replacements.TryGetValue(word, out alternative);
        }

        private void ReadIdioms()
        {
            var stream = new DictionaryStream(Path.Combine(resourcesPath, "IdiomLookupTable.txt"), new FileStreamSource());
            idiomsSentiment = stream.ReadDataFromStream(int.Parse).ToDictionary(item => item.Word, item => item.Value, StringComparer.OrdinalIgnoreCase);
            foreach (var item in idiomsSentiment.ToArray())
            {
                var arr = item.Key.Where(c => (char.IsLetterOrDigit(c) || c == '-')).ToArray();
                var alternative = new string(arr);
                replacements[item.Key] = alternative;
                idiomsSentiment[alternative] = item.Value;
            }
        }

        private void ReadSlang()
        {
            var stream = new DictionaryStream(Path.Combine(resourcesPath, "SlangLookupTable.txt"), new FileStreamSource());
            foreach (var item in stream.ReadDataFromStream(item => item))
            {
                replacements[item.Word] = item.Value;
            }
        }
    }
}
