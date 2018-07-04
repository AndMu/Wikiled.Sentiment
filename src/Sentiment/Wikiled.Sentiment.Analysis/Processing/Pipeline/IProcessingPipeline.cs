using System;
using System.Threading;
using Wikiled.Common.Logging;

namespace Wikiled.Sentiment.Analysis.Processing.Pipeline
{
    public interface IProcessingPipeline
    {
        ISplitterHelper Splitter { get; }

        PerformanceMonitor Monitor { get; }

        IObservable<ProcessingContext> ProcessStep();

        SemaphoreSlim ProcessingSemaphore { get; set; }
    }
}