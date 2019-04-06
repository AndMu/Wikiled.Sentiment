using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.Arff.Logic;
using Wikiled.Common.Extensions;
using Wikiled.Sentiment.Analysis.Containers;
using Wikiled.Sentiment.Analysis.Pipeline;
using Wikiled.Sentiment.Analysis.Processing.Persistency;
using Wikiled.Sentiment.ConsoleApp.Analysis.Config;
using Wikiled.Sentiment.ConsoleApp.Analysis.Wikiled.Sentiment.ConsoleApp.Extraction.Bootstrap.Data;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.ConsoleApp.Analysis
{
    public class BoostrapCommand : BaseRawCommand<BootsrapConfig>
    {
        private string negativeResult;

        private string positiveResult;

        public BoostrapCommand(ILogger<BoostrapCommand> log, BootsrapConfig config, IDataLoader loader, ISessionContainer container)
            : base(log, config, loader, container)
        {
            Semaphore = new SemaphoreSlim(3000);
        }

        protected override async Task Process(IObservable<IParsedDocumentHolder> reviews,
                                              ISessionContainer container,
                                              ISentimentDataHolder sentimentAdjustment)
        {
            var client = container.GetTesting(Config.Model);
            container.Context.Lexicon = sentimentAdjustment;
            using (Observable.Interval(TimeSpan.FromSeconds(30))
                .Subscribe(item => Logger.LogInformation(client.Pipeline.Monitor.ToString())))
            {
                client.TrackArff = false;
                client.UseBagOfWords = Config.UseBagOfWords;
                client.Init();
                var result = await client.Process(reviews.ObserveOn(TaskPoolScheduler.Default))
                    .Select(
                        item =>
                        {
                            Semaphore.Release();
                            return Resolve(item);
                        })
                    .ToArray();
                SaveDocuments(result);
            }
        }

        private EvalData Resolve(ProcessingContext context)
        {
            var rating = context.Adjustment.Rating;
            var data = new EvalData(context.Processed.Id, context.Review.Text);
            data.Stars = rating.StarsRating;
            var bootSentimentValue = context.Review.CalculateRawRating();
            var bootAllSentiments = context.Review.GetAllSentiments().Where(item => !item.Owner.IsInvertor || item.Owner.IsSentiment).ToArray();

            if (bootSentimentValue.StarsRating.HasValue)
            {
                data.Stars = bootSentimentValue.StarsRating.Value;
                data.TotalSentiments = bootAllSentiments.Length;
            }

            return data;
        }

        private void SaveDocuments(EvalData[] context)
        {
            Logger.LogInformation("SaveDocuments");
            InitOutput();
            var types = context.Where(item => item.Stars.HasValue && item.TotalSentiments >= Config.Minimum).ToArray();
            var positive = types.Count(item => item.CalculatedPositivity == PositivityType.Positive);
            var negative = types.Count(item => item.CalculatedPositivity == PositivityType.Negative);
            var neutral = types.Count(item => item.CalculatedPositivity == PositivityType.Neutral);
            Logger.LogInformation($"Total (Positive): {positive} Total (Negative): {negative} Total (Neutral): {neutral}");

            if (Config.BalancedTop.HasValue)
            {
                var cutoff = positive > negative ? negative : positive;

                cutoff = (int)(Config.BalancedTop.Value * cutoff);
                IEnumerable<EvalData> negativeItems = types.OrderBy(item => item.Stars).ThenByDescending(item => item.TotalSentiments).Take(cutoff);
                IEnumerable<EvalData> positiveItems = types.OrderByDescending(item => item.Stars).ThenByDescending(item => item.TotalSentiments).Take(cutoff);
                IEnumerable<EvalData> select = negativeItems.Union(positiveItems);
                
                types = select.OrderBy(item => Guid.NewGuid()).ToArray();
                Logger.LogInformation($"After balancing took: {cutoff}");
            }

            foreach (var data in types)
            {
                SaveDocument(data);
            }
        }

        private void SaveDocument(EvalData context)
        {
            string place = context.CalculatedPositivity == PositivityType.Positive ? positiveResult : negativeResult;
            var file = Path.Combine(place, context.Id + ".txt");
            if (File.Exists(file))
            {
                throw new ApplicationException($"File already exist: {file}");
            }

            File.WriteAllText(file, context.Text);
        }

        private void InitOutput()
        {
            positiveResult = Path.Combine(Config.Destination, "pos");
            negativeResult = Path.Combine(Config.Destination, "neg");
            if (Directory.Exists(negativeResult))
            {
                Directory.Delete(negativeResult, true);
            }

            if (Directory.Exists(positiveResult))
            {
                Directory.Delete(positiveResult, true);
            }

            negativeResult.EnsureDirectoryExistence();
            positiveResult.EnsureDirectoryExistence();
        }
    }
}