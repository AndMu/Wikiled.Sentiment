using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Wikiled.Common.Logging;
using Wikiled.Common.Serialization;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public static class TextSplitterExtension
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger("TextSplitterExtension");

        public static IEnumerable<IParsedDocumentHolder> GetParsedReviewHolders(this ITextSplitter splitter, string path, bool? positive)
        {
            log.LogInformation("Reading: {0}", path);
            if (string.IsNullOrEmpty(path))
            {
                log.LogWarning("One of paths is empty");
                return new IParsedDocumentHolder[] { };
            }

            double? stars = null;
            if (positive == true)
            {
                stars = 5;
            }
            else if (positive == false)
            {
                stars = 1;
            }

            return GetReview(splitter, path, stars);
        }

        public static IObservable<IParsedDocumentHolder> GetParsedReviewHolders(this ITextSplitter splitter, IProcessingData data)
        {
            IObservable<ParsingDocumentHolder> all = data.All.Select(
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

        private static IEnumerable<IParsedDocumentHolder> GetReview(ITextSplitter splitter, string path, double? stars)
        {
            if (File.Exists(path))
            {
                foreach (var line in File.ReadLines(path))
                {
                    yield return new ParsingDocumentHolder(splitter,
                                                           new Document(line.SanitizeXmlString()) { Stars = stars });
                }
            }
            else
            {
                foreach (var file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
                {
                    yield return new AsyncParsingDocumentHolder(Task.Run(() =>
                    {
                        var fileInfo = new FileInfo(file);
                        var document = new Document(File.ReadAllText(file).SanitizeXmlString())
                        {
                            Id = $"{fileInfo.Directory.Name}_{Path.GetFileNameWithoutExtension(fileInfo.Name)}",
                            Stars = stars
                        };

                        return (IParsedDocumentHolder)new ParsingDocumentHolder(splitter, document);
                    }));
                }
            }
        }
    }
}
