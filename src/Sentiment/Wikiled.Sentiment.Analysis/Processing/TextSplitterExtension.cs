using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Wikiled.Common.Logging;
using Wikiled.Common.Serialization;
using Wikiled.Sentiment.Analysis.Containers;
using Wikiled.Sentiment.Analysis.Processing.Persistency;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.NLP.Repair;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public static class TextSplitterExtension
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger("TextSplitterExtension");

        public static IEnumerable<IParsedDocumentHolder> GetParsedReviewHolders(this ISessionContainer container, string path, bool? positive)
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

            return GetReview(container, path, stars);
        }

        public static IObservable<IParsedDocumentHolder> GetParsedReviewHolders(this ISessionContainer container, IDataSource data)
        {
            var all = data.Load()
                          .Select(processingData
                                      => (IParsedDocumentHolder)new AsyncParsingDocumentHolder(
                                          ConstructHolder(container, processingData)));

            return all;
        }

        private static async Task<IParsedDocumentHolder> ConstructHolder(ISessionContainer container, DataPair processingData)
        {
            var result = await processingData.Data.ConfigureAwait(false);

            if (processingData.Sentiment == SentimentClass.Positive)
            {
                SetStars(result, 5);
            }
            else if (processingData.Sentiment == SentimentClass.Negative)
            {
                SetStars(result, 1);
            }

            return new ParsingDocumentHolder(container.GetTextSplitter(),
                                             container.GetWordFactory(),
                                             container.Resolve<IContextSentenceRepairHandler>(),
                                             result);
        }

        private static void SetStars(SingleProcessingData processingData, double defaultStars)
        {
            if (processingData.Stars == null)
            {
                processingData.Stars = defaultStars;
            }
        }

        private static IEnumerable<IParsedDocumentHolder> GetReview(ISessionContainer container, string path, double? stars)
        {
            if (File.Exists(path))
            {
                foreach (var line in File.ReadLines(path))
                {
                    yield return new ParsingDocumentHolder(container.GetTextSplitter(),
                                                           container.GetWordFactory(),
                                                           container.Resolve<IContextSentenceRepairHandler>(),
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

                        return (IParsedDocumentHolder)new ParsingDocumentHolder(container.GetTextSplitter(),
                                                                                 container.GetWordFactory(),
                                                                                 container.Resolve<IContextSentenceRepairHandler>(),
                                                                                 document);
                    }));
                }
            }
        }
    }
}
