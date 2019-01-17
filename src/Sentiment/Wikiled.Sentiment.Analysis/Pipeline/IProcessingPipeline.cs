using System;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Pipeline
{
    public interface IProcessingPipeline
    {
        PerformanceMonitor Monitor { get; }

        void ResetMonitor();

        IObservable<ProcessingContext> ProcessStep(IObservable<IParsedDocumentHolder> reviews);
    }
}