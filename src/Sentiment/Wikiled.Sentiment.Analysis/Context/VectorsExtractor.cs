using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Wikiled.Common.Extensions;
using Wikiled.Sentiment.Text.Async;
using Wikiled.Sentiment.Text.Helpers;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Context
{
    public class VectorsExtractor
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, WordVectorsData> wordVectors = new Dictionary<string, WordVectorsData>(StringComparer.OrdinalIgnoreCase);

        public VectorsExtractor(int windowSize)
        {
            WindowSize = windowSize;
        }

        public int WindowSize { get; }

        public ICollection<WordVectorsData> WordVectors => wordVectors.Values;

        public void Process(Document document)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            AutoEvictingDictionary<WordEx, WordsContext> table =
                new AutoEvictingDictionary<WordEx, WordsContext>(length: WindowSize);
            AutoEvictingDictionary<SentenceItem, SentenceItem> sentences =
                new AutoEvictingDictionary<SentenceItem, SentenceItem>(length: WindowSize);
            foreach(var sentenceItem in document.Sentences)
            {
                table.Increment();
                sentences.Increment();
                sentences.Add(sentenceItem, sentenceItem);
                foreach(var word in sentenceItem.Words)
                {
                    WordsContext current = GetVector(word).CreateNewVector();
                    current.SentimentValue = sentences.Values.Select(item => item.CalculateSentiment().RawRating).Where(item => item.HasValue).Select(item => item.Value).Sum();
                    foreach(var addedRecords in table.Values)
                    {
                        addedRecords.AddContext(word);
                        current.AddContext(addedRecords.Word);
                    }

                    table[word] = current;
                }
            }
        }

        public void Save(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(path));
            }

            log.Info("Saving Vectors to: {0}", path);
            path.EnsureDirectoryExistence();
            Parallel.ForEach(
                WordVectors.Where(item => item.Word.Value > 0),
                AsyncSettings.DefaultParallel,
                vector => vector.Save(path));
        }

        private WordVectorsData GetVector(WordEx word)
        {
            WordVectorsData vectors = wordVectors.GetSafeCreate(word.Text, () => new WordVectorsData(word));
            vectors.CreateNewVector();
            return vectors;
        }
    }
}
