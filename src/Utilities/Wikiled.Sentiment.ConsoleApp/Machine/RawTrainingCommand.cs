using System;
using NLog;
using Wikiled.Sentiment.Analysis.CrossDomain;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.ConsoleApp.Machine
{
    /// <summary>
    /// raw -Articles="C:\Cloud\OneDrive\Study\Medical\articles.xml" -Redis [-Features=c:\out\features_my.xml] [-Weights=c:\out\trumpWeights.csv] [-FullWeightReset]
    /// </summary>
    internal class RawTrainingCommand : BaseRawCommand
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public string Features { get; set; }

        public string Weights { get; set; }

        public bool FullWeightReset { get; set; }

        public bool UseAll { get; set; }

        protected override void Process(IObservable<IParsedDocumentHolder> reviews, ISplitterHelper splitter)
        {
            log.Info("Training Operation...");
            TrainingClient client = new TrainingClient(splitter, reviews, @".\Svm");
            client.OverrideFeatures = Features;
            client.UseBagOfWords = UseBagOfWords;
            client.UseAll = UseAll;

            if (!string.IsNullOrEmpty(Weights))
            {
                log.Info("Adjusting Word2Vec sentiments...");
                if (FullWeightReset)
                {
                    log.Info("Full weight reset");
                    splitter.DataLoader.SentimentDataHolder.Clear();
                }

                var adjuster = new WeightSentimentAdjuster(splitter.DataLoader.SentimentDataHolder);
                adjuster.Adjust(Weights);
            }

            client.Train().Wait();
        }
    }
}