using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public static class TextSplitterExtension
    {
        public static IObservable<IParsedDocumentHolder> GetParsedReviewHolders(this ITextSplitter splitter, string path, bool positive)
        {
            return Observable.Create<IParsedDocumentHolder>(
                observer =>
                {
                    foreach (var line in GetReview(path))
                    {
                        var item = new SingleProcessingData(new Document(line));
                        if (positive)
                        {
                            item.Stars = 5;
                            item.Document.Stars = 5;
                        }
                        else
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

                    observer.OnCompleted();
                    return Disposable.Empty;
                });
        }

        private static IEnumerable<string> GetReview(string path)
        {
            if (File.Exists(path))
            {
                foreach (var line in File.ReadLines(path))
                {
                    yield return line;
                }
            }
            else
            {
                foreach (var file in Directory.EnumerateFiles(path))
                {
                    yield return File.ReadAllText(file);
                }
            }
        }
    }
}
