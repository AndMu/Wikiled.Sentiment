using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Wikiled.Sentiment.Analysis.Containers;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.ConsoleApp.Analysis.Config;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.ConsoleApp.Analysis
{
    /// <summary>
    /// train -Articles="C:\Cloud\OneDrive\Study\Medical\articles.xml" -Redis [-Features=c:\out\features_my.xml] [-Weights=c:\out\trumpWeights.csv] [-FullWeightReset]
    /// </summary>
    [Description("pSenti training command")]
    internal class TrainCommand : BaseRawCommand<TrainingConfig>
    {
        public TrainCommand(ILogger<TrainCommand> log, TrainingConfig config, ISessionContainer container)
            : base(log, config, container)
        {
        }

        protected override async Task Process(IObservable<IParsedDocumentHolder> reviews, ISessionContainer container, ISentimentDataHolder sentimentAdjustment)
        {
            Logger.LogInformation("Training Operation...");
            ITrainingClient client = container.GetTraining(Config.Model);
            container.Context.Lexicon = sentimentAdjustment;
            client.OverrideAspects = Config.Features;
            client.UseBagOfWords = Config.UseBagOfWords;
            client.UseAll = Config.UseAll;
            await client.Train(reviews.ObserveOn(TaskPoolScheduler.Default)).ConfigureAwait(false);
        }
    }
}