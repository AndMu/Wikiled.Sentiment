using System;
using System.Threading;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Processing.Pipeline
{
    public interface IProcessingPipeline
    {
        ISplitterHelper Splitter { get; }

        PerformanceMonitor Monitor { get; }

        IObservable<ProcessingContext> ProcessStep(IObservable<IParsedDocumentHolder> reviews);

        SemaphoreSlim ProcessingSemaphore { get; set; }
    }
}