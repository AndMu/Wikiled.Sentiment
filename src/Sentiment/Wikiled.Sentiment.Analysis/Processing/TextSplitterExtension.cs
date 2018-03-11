using System.Collections.Generic;
using System.IO;
using NLog;
using Wikiled.Common.Serialization;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public static class TextSplitterExtension
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static IEnumerable<IParsedDocumentHolder> GetParsedReviewHolders(this ITextSplitter splitter, string path, bool? positive)
        {
            log.Info("Reading: {0}", path);
            if (string.IsNullOrEmpty(path))
            {
                log.Warn("One of paths is empty");
                yield break;
            }

            foreach (var document in GetReview(path))
            {
                var item = new SingleProcessingData(document);
                if (positive == true)
                {
                    item.Stars = 5;
                    item.Document.Stars = 5;
                }
                else if (positive == false)
                {
                    item.Stars = 1;
                    item.Document.Stars = 1;
                }

                yield return new ParsingDocumentHolder(splitter, item);
            }
        }

        public static IEnumerable<IParsedDocumentHolder> GetParsedReviewHolders(this ITextSplitter splitter, ProcessingData data)
        {
            foreach (var processingData in data.Positive)
            {
                SetStars(processingData, 5);
                yield return new ParsingDocumentHolder(splitter, processingData);
            }

            foreach (var processingData in data.Negative)
            {
                SetStars(processingData, 1);
                yield return new ParsingDocumentHolder(splitter, processingData);
            }

            foreach (var processingData in data.Neutral)
            {
                yield return new ParsingDocumentHolder(splitter, processingData);
            }
        }

        private static void SetStars(SingleProcessingData processingData, double defaultStars)
        {
            if (processingData.Document.Stars == null)
            {
                processingData.Stars = defaultStars;
                processingData.Document.Stars = defaultStars;
            }
            else
            {
                processingData.Stars = processingData.Document.Stars.Value;
            }
        }

        private static IEnumerable<Document> GetReview(string path)
        {
            if (File.Exists(path))
            {
                foreach (var line in File.ReadLines(path))
                {
                    yield return new Document(line.SanitizeXmlString());
                }
            }
            else
            {
                foreach (var file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
                {
                    FileInfo fileInfo = new FileInfo(file);
                    yield return new Document(File.ReadAllText(file).SanitizeXmlString())
                                     {
                                         Id = $"{fileInfo.Directory.Name}_{Path.GetFileNameWithoutExtension(fileInfo.Name)}"
                                     };
                }
            }
        }
    }
}
