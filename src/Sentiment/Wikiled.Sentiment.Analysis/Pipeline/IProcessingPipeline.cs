using System;
using System.Threading;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Analysis.Pipeline
{
    public interface IProcessingPipeline
    {
        PerformanceMonitor Monitor { get; }

        IObservable<ProcessingContext> ProcessStep(IObservable<IParsedDocumentHolder> reviews);

        SemaphoreSlim ProcessingSemaphore { get; set; }

        ISentimentDataHolder LexiconAdjustment { get; set; }
    }
}