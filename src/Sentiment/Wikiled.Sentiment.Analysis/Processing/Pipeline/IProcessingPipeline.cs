using System;

namespace Wikiled.Sentiment.Analysis.Processing.Pipeline
{
    public interface IProcessingPipeline
    {
        ISplitterHelper Splitter { get; }

        IObservable<ProcessingContext> ProcessStep();
    }
}