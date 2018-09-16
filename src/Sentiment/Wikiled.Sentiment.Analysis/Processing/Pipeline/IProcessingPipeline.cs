using System;
using System.Threading;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.Analysis.Processing.Splitters;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Processing.Pipeline
{
    public interface IProcessingPipeline
    {
        IContainerHelper ContainerHolder { get; }

        PerformanceMonitor Monitor { get; }

        IObservable<ProcessingContext> ProcessStep(IObservable<IParsedDocumentHolder> reviews);

        SemaphoreSlim ProcessingSemaphore { get; set; }
    }
}