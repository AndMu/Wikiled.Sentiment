using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using Wikiled.Arff.Logic;
using Wikiled.Common.Extensions;
using Wikiled.Common.Logging;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Context
{
    public class WordVectorsData
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<WordVectorsData>();

        public WordVectorsData(WordEx word)
        {
            Word = word ?? throw new ArgumentNullException(nameof(word));
            Vectors = new List<WordsContext>();
        }

        public WordsContext CurrentVector => Vectors.Count == 0
                                                 ? null
                                                 : Vectors[Vectors.Count - 1];

        public IList<WordsContext> Vectors { get; }

        public WordEx Word { get; }

        public WordsContext CreateNewVector()
        {
            if (Vectors.Count != 0 &&
                CurrentVector.Words.Count == 0)
            {
                log.LogDebug("Ignoring call to create vector - current is empty");
                return CurrentVector;
            }

            var vector = new WordsContext(Word);
            Vectors.Add(vector);
            return vector;
        }

        public void Save(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            }

            log.LogInformation("Saving {0}...", path);
            var fileName = $"{Word.Text.CreatePureLetterText()}.arff";
            path = Path.Combine(path, fileName);
            IArffDataSet arff = ArffDataSet.Create<PositivityType>(Word.Text);
            arff.UseTotal = true;
            foreach (WordsContext vector in Vectors)
            {
                IArffDataRow review = arff.AddDocument();
                review.Class.Value = vector.SentimentValue > 0
                    ? PositivityType.Positive
                    : PositivityType.Negative;
                foreach (WordEx wordItem in vector.Words)
                {
                    if (!wordItem.IsAspect &&
                        wordItem.Value == 0)
                    {
                        continue;
                    }

                    DataRecord addedWord = review.AddRecord(wordItem.Text);
                    addedWord.Value = addedWord.Total;
                }
            }

            arff.Save(path);
            log.LogInformation("Saving {0} Completed.", path);
        }
    }
}