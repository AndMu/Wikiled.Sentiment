using System;
using Wikiled.MachineLearning.Mathematics;
using Wikiled.Sentiment.Analysis.Pipeline;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Text.Analysis.NLP.NRC;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public interface ITestingClient
    {
        IProcessingPipeline Pipeline { get; }

        string AspectPath { get; set; }

        AspectSentimentTracker AspectSentiment { get; }

        bool DisableAspects { get; set; }

        bool DisableSvm { get; set; }

        int Errors { get; }

        IMachineSentiment MachineSentiment { get; }

        PrecisionRecallCalculator<bool> Performance { get; }

        SentimentVector SentimentVector { get; }

        string SvmPath { get; }

        bool TrackArff { get; set; }

        bool UseBagOfWords { get; set; }

        string GetPerformanceDescription();

        void Init();

        IObservable<ProcessingContext> Process(IObservable<IParsedDocumentHolder> reviews);

        void Save(string path);
    }
}