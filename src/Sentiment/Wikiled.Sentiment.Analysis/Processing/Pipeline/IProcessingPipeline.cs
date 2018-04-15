using System;
using Wikiled.Common.Logging;

namespace Wikiled.Sentiment.Analysis.Processing.Pipeline
{
    public interface IProcessingPipeline
    {
        ISplitterHelper Splitter { get; }

        PerformanceMonitor Monitor { get; }

        IObservable<ProcessingContext> ProcessStep();
    }
}