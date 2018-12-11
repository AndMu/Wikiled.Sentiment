using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.Analysis.Containers;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.ConsoleApp.Analysis
{
    /// <summary>
    /// train -Articles="C:\Cloud\OneDrive\Study\Medical\articles.xml" -Redis [-Features=c:\out\features_my.xml] [-Weights=c:\out\trumpWeights.csv] [-FullWeightReset]
    /// </summary>
    [Description("pSenti training command")]
    internal class TrainCommand : BaseRawCommand
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<TrainCommand>();

        /// <summary>
        /// Path to Feautres/Aspects
        /// </summary>
        public string Features { get; set; }

        /// <summary>
        /// Do you want to use all words of filter using threshold (min 3 reviews with words and words with 10 occurrences)
        /// </summary>
        public bool UseAll { get; set; }

        public string Model { get; set; } = @".\Svm";

        protected override void Process(IObservable<IParsedDocumentHolder> reviews, ISessionContainer container, ISentimentDataHolder sentimentAdjustment)
        {
            log.LogInformation("Training Operation...");
            ITrainingClient client = container.GetTraining(Model);
            container.Context.Lexicon = sentimentAdjustment;
            client.OverrideAspects = Features;
            client.UseBagOfWords = UseBagOfWords;
            client.UseAll = UseAll;
            client.Train(reviews.ObserveOn(TaskPoolScheduler.Default)).Wait();
        }
    }
}