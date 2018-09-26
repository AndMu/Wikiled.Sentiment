﻿using System;
using System.Threading.Tasks;
using Wikiled.Sentiment.Analysis.Pipeline;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Text.Analysis.NLP.NRC;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public interface ITrainingClient
    {
        IProcessingPipeline Pipeline { get; }

        bool DisableAspects { get; set; }

        string OverrideAspects { get; set; }

        SentimentVector SentimentVector { get; }

        bool UseAll { get; set; }

        bool UseBagOfWords { get; set; }

        Task Train(IObservable<IParsedDocumentHolder> reviews);
    }
}