using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
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
                var item = new SingleProcessingData(document.Text);
                if (positive == true)
                {
                    item.Stars = 5;
                }
                else if (positive == false)
                {
                    item.Stars = 1;
                }

                yield return new ParsingDocumentHolder(splitter, document);
            }
        }

        public static IObservable<IParsedDocumentHolder> GetParsedReviewHolders(this ITextSplitter splitter, IProcessingData data)
        {
            var all = data.All.Select(
                processingData =>
                {
                    if (processingData.Sentiment == SentimentClass.Positive)
                    {
                        SetStars(processingData.Data, 5);
                    }
                    else if (processingData.Sentiment == SentimentClass.Negative)
                    {
                        SetStars(processingData.Data, 1);
                    }

                    return new ParsingDocumentHolder(splitter, processingData.Data);
                });

            return all;
        }

        private static void SetStars(SingleProcessingData processingData, double defaultStars)
        {
            if (processingData.Stars == null)
            {
                processingData.Stars = defaultStars;
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
