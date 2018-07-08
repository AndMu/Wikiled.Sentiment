using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using NLog;
using Wikiled.Arff.Persistence;
using Wikiled.Common.Logging;
using Wikiled.Console.Arguments;
using Wikiled.MachineLearning.Mathematics;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Splitters;
using Wikiled.Sentiment.ConsoleApp.Extraction.Bootstrap.Data;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.ConsoleApp.Extraction.Bootstrap
{
    public abstract class BaseBoostrapCommand : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(Environment.ProcessorCount / 2);

        private ISplitterHelper bootStrapSplitter;

        private ParsedReviewManagerFactory managerFactory = new ParsedReviewManagerFactory();

        private PerformanceMonitor monitor;

        private ISplitterHelper defaultSplitter;

        private PrecisionRecallCalculator<bool> performance;

        private PrecisionRecallCalculator<bool> performanceSub;

        [Description("Minimum sentiment words occurence")]
        public int Minimum { get; set; } = 3;

        [Required]
        public string Destination { get; set; }

        public bool InvertOff { get; set; }

        public bool Neutral { get; set; }

        public double? BalancedTop { get; set; }

        [Required]
        public string Path { get; set; }

        [Required]
        public string Words { get; set; }

        protected override async Task Execute(CancellationToken token)
        {
            List<Task> initTasks = new List<Task>();
            initTasks.Add(Task.Run(() => LoadBootstrap()));
            if (Neutral)
            {
                initTasks.Add(Task.Run(() => LoadDefault()));
            }

            await Task.WhenAll(initTasks);
            monitor = new PerformanceMonitor(0);
            EvalData[] types;
            using (Observable.Interval(TimeSpan.FromSeconds(30))
                             .Subscribe(item => log.Info(monitor)))
            {
                var reviews = ReadFiles();
                var subscriptionMessage = reviews.Concat()
                                                 .ObserveOn(TaskPoolScheduler.Default)
                                                 .Select(item => Observable.Start(() => ProcessReview(item), TaskPoolScheduler.Default))
                                                 .Merge()
                                                 .Merge()
                                                 .Where(item => item != null)
                                                 .Where(item => item.Stars == 5 || item.Stars == 1 || item.IsNeutral == true);

                performance = new PrecisionRecallCalculator<bool>();
                performanceSub = new PrecisionRecallCalculator<bool>();
                types = subscriptionMessage.ToEnumerable().Where(item => item.Stars.HasValue && item.TotalSentiments >= Minimum).ToArray();
                var positive = types.Count(item => item.CalculatedPositivity == PositivityType.Positive);
                var negative = types.Count(item => item.CalculatedPositivity == PositivityType.Negative);
                var neutral = types.Count(item => item.CalculatedPositivity == PositivityType.Neutral);
                var positiveOrg = types.Count(item => item.Original == PositivityType.Positive);
                var negativeOrg = types.Count(item => item.Original == PositivityType.Negative);
                var neutralOrg = types.Count(item => item.Original == PositivityType.Neutral);
                log.Info($"Total (Positive): {positive}({positiveOrg}) Total (Negative): {negative}({negativeOrg}) Total (Neutral): {neutral}({neutralOrg})");
                if (BalancedTop.HasValue)
                {
                    var cutoff = positive > negative ? negative : positive;
                    if (Neutral)
                    {
                        cutoff = cutoff > neutral ? neutral : cutoff;
                    }

                    cutoff = (int)(BalancedTop.Value * cutoff);
                    var negativeItems = types.OrderBy(item => item.Stars).ThenByDescending(item => item.TotalSentiments).Take(cutoff);
                    var positiveItems = types.OrderByDescending(item => item.Stars).ThenByDescending(item => item.TotalSentiments).Take(cutoff);
                    var select = negativeItems.Union(positiveItems);
                    if (Neutral)
                    {
                        var neutralItems = types.Where(item => item.IsNeutral == true).OrderBy(item => Guid.NewGuid()).Take(cutoff);
                        select = select.Union(neutralItems);
                    }

                    types = select.OrderBy(item => Guid.NewGuid()).ToArray();
                    log.Info($"After balancing took: {cutoff}");
                }

                foreach (var item in types)
                {
                    if (item.Original.HasValue &&
                        item.Original.Value != PositivityType.Neutral &&
                        item.IsNeutral != true)
                    {
                        performance.Add(item.Original == PositivityType.Positive, item.CalculatedPositivity == PositivityType.Positive);
                    }

                    performanceSub.Add(item.Original == PositivityType.Neutral, item.CalculatedPositivity == PositivityType.Neutral);
                }

                log.Info($"{performance.GetTotalAccuracy()} Precision (true): {performance.GetPrecision(true)} Precision (false): {performance.GetPrecision(false)}");
                if (Neutral)
                {
                    log.Info($"{performanceSub.GetTotalAccuracy()} Precision (true - Subjective): {performanceSub.GetPrecision(true)} Precision (false - Subjective): {performanceSub.GetPrecision(false)}");
                }

            }

            log.Info("Saving results...");
            SaveResult(types);
        }

        protected abstract IObservable<EvalData> GetDataPacket(string file);

        protected abstract void SaveResult(EvalData[] subscriptionMessage);

        private void LoadBootstrap()
        {
            log.Info("Loading text splitter for bootstrapping");
            var config = new ConfigurationHandler();
            var splitterFactory = new MainSplitterFactory(
                new LocalCacheFactory(new MemoryCache(new MemoryCacheOptions())),
                config);
            bootStrapSplitter = splitterFactory.Create(POSTaggerType.SharpNLP);
            log.Info("Removing default lexicon");
            bootStrapSplitter.DataLoader.SentimentDataHolder.Clear();
            bootStrapSplitter.DataLoader.DisableFeatureSentiment = InvertOff;
            var adjuster = new WeightSentimentAdjuster(bootStrapSplitter.DataLoader.SentimentDataHolder);
            adjuster.Adjust(Words);
        }

        private void LoadDefault()
        {
            log.Info("Loading default text splitter");
            var config = new ConfigurationHandler();
            var splitterFactory = new MainSplitterFactory(new LocalCacheFactory(new MemoryCache(new MemoryCacheOptions())), config);
            defaultSplitter = splitterFactory.Create(POSTaggerType.SharpNLP);
        }

        private async Task<EvalData> ProcessReview(EvalData data)
        {
            await semaphore.WaitAsync()
                           .ConfigureAwait(false);
            try
            {
                var original = await bootStrapSplitter.Splitter.Process(new ParseRequest(data.Text)).ConfigureAwait(false);
                var bootReview = managerFactory.Create(bootStrapSplitter.DataLoader, original).Create();

                var bootSentimentValue = bootReview.CalculateRawRating();
                var bootAllSentiments = bootReview.GetAllSentiments().Where(item => !item.Owner.IsInvertor || item.Owner.IsSentiment).ToArray();

                if (bootSentimentValue.StarsRating.HasValue)
                {
                    data.Stars = bootSentimentValue.StarsRating.Value;
                    data.TotalSentiments = bootAllSentiments.Length;
                    return data;
                }
                else if (Neutral)
                {
                    // check also using default lexicon
                    var main = await defaultSplitter.Splitter.Process(new ParseRequest(data.Text))
                                                    .ConfigureAwait(false);
                    var originalReview = managerFactory.Create(bootStrapSplitter.DataLoader, main).Create();
                    var originalRating = originalReview.CalculateRawRating();

                    // main.GetReview().Items.SelectMany(item => item.Inquirer.Records).Where(item=>  item.Description.Harward.)
                    if (!originalRating.StarsRating.HasValue)
                    {
                        data.IsNeutral = true;
                        return data;
                    }
                }

                return null;
            }
            finally
            {
                monitor.Increment();
                semaphore.Release();
            }
        }

        private IEnumerable<IObservable<EvalData>> ReadFiles()
        {
            if (File.Exists(Path))
            {
                yield return ReadFile(Path);
            }
            else
            {
                foreach (var file in Directory.GetFiles(Path, "*.*", SearchOption.AllDirectories))
                {
                    yield return ReadFile(file);
                }
            }
        }

        private IObservable<EvalData> ReadFile(string file)
        {
            return GetDataPacket(file)
                .Where(item => item != null)
                .Select(
                    semEvalData =>
                    {
                        monitor.ManualyCount();
                        return semEvalData;
                    });
        }
    }
}
