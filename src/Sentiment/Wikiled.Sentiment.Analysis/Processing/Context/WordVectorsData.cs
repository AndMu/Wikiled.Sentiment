using System.Collections.Generic;
using System.IO;
using NLog;
using Wikiled.Arff.Persistence;
using Wikiled.Common.Arguments;
using Wikiled.Common.Extensions;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Processing.Context
{
    public class WordVectorsData
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public WordVectorsData(WordEx word)
        {
            Guard.NotNull(() => word, word);
            Word = word;
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
                log.Debug("Ignoring call to create vector - current is empty");
                return CurrentVector;
            }

            var vector = new WordsContext(Word);
            Vectors.Add(vector);
            return vector;
        }

        public void Save(string path)
        {
            Guard.NotNullOrEmpty(() => path, path);
            log.Info("Saving {0}...", path);
            string fileName = $"{Word.Text.CreatePureLetterText()}.arff";
            path = Path.Combine(path, fileName);
            IArffDataSet arff = ArffDataSet.Create<PositivityType>(Word.Text);
            arff.UseTotal = true;
            foreach (var vector in Vectors)
            {
                IArffDataRow review = arff.AddDocument();
                review.Class.Value = vector.SentimentValue > 0
                    ? PositivityType.Positive
                    : PositivityType.Negative;
                foreach (var wordItem in vector.Words)
                {
                    if (!wordItem.IsAspect &&
                        wordItem.Value == 0)
                    {
                        continue;
                    }

                    var addedWord = review.AddRecord(wordItem.Text);
                    addedWord.Value = addedWord.Total;
                }
            }

            arff.Save(path);
            log.Info("Saving {0} Completed.", path);
        }
    }
}