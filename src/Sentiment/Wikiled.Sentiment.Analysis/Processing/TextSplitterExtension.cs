using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using NLog;
using Wikiled.Core.Utility.Serialization;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public static class TextSplitterExtension
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static IObservable<IParsedDocumentHolder> GetParsedReviewHolders(this ITextSplitter splitter, string path, bool? positive)
        {
            if (string.IsNullOrEmpty(path))
            {
                log.Warn("One of paths is empty");
                return Observable.Empty<IParsedDocumentHolder>();
            }

            return Observable.Create<IParsedDocumentHolder>(
                observer =>
                {
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
                        
                        observer.OnNext(new ParsingDocumentHolder(splitter, item));
                    }

                    observer.OnCompleted();
                    return Disposable.Empty;
                });
        }

        public static IObservable<IParsedDocumentHolder> GetParsedReviewHolders(this ITextSplitter splitter, ProcessingData data)
        {
            return Observable.Create<IParsedDocumentHolder>(
                observer =>
                {
                    foreach (var processingData in data.Positive)
                    {
                        processingData.Stars = 5;
                        observer.OnNext(new ParsingDocumentHolder(splitter, processingData));
                    }

                    foreach (var processingData in data.Negative)
                    {
                        processingData.Stars = 1;
                        observer.OnNext(new ParsingDocumentHolder(splitter, processingData));
                    }

                    foreach (var processingData in data.Neutral)
                    {
                        observer.OnNext(new ParsingDocumentHolder(splitter, processingData));
                    }

                    observer.OnCompleted();
                    return Disposable.Empty;
                });
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
                foreach (var file in Directory.EnumerateFiles(path))
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
