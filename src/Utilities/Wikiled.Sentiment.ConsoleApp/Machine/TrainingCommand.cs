using System;
using NLog;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.ConsoleApp.Machine
{
    /// <summary>
    /// train -Articles="C:\Cloud\OneDrive\Study\Medical\articles.xml" -Redis [-Features=c:\out\features_my.xml] [-Weights=c:\out\trumpWeights.csv] [-FullWeightReset]
    /// </summary>
    internal class TrainingCommand : BaseRawCommand
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Path to Feautres/Aspects
        /// </summary>
        public string Features { get; set; }

        /// <summary>
        /// Do you want to use all words of filter using threshold (min 3 reviews with words and words with 10 occurrences)
        /// </summary>
        public bool UseAll { get; set; }

        protected override void Process(IObservable<IParsedDocumentHolder> reviews, ISplitterHelper splitter)
        {
            log.Info("Training Operation...");
            TrainingClient client = new TrainingClient(splitter, reviews, @".\Svm");
            client.OverrideAspects = Features;
            client.UseBagOfWords = UseBagOfWords;
            client.UseAll = UseAll;
            client.Train().Wait();
        }
    }
}