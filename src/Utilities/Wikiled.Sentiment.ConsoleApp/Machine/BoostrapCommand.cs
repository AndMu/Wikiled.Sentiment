using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Wikiled.Arff.Persistence;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Logging;
using Wikiled.Core.Utility.Resources;
using Wikiled.MachineLearning.Mathematics;
using Wikiled.Sentiment.Analysis.CrossDomain;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Splitters;
using Wikiled.Sentiment.ConsoleApp.Machine.Data;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.Twitter;

namespace Wikiled.Sentiment.ConsoleApp.Machine
{
    /// <summary>
    ///     boot -Words=words.csv -Path="E:\DataSets\SemEval\All\out\ -Destination=c:\DataSets\SemEval\train.txt
    /// </summary>
    public class BoostrapCommand : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly MessageCleanup cleanup = new MessageCleanup();

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(Environment.ProcessorCount / 2);

        private ISplitterHelper bootStrapSplitter;

        private Dictionary<string, string> exist;

        private PerformanceMonitor monitor;

        private PrecisionRecallCalculator<bool> performance;

        [Required]
        public string Destination { get; set; }

        public int Minimum { get; set; } = 3;

        public bool UseInvert { get; set; }

        public double? BalancedTop { get; set; }

        [Required]
        public string Path { get; set; }

        [Required]
        public string Words { get; set; }

        public override void Execute()
        {
            LoadBootstrap();
            monitor = new PerformanceMonitor(0);
            exist = new Dictionary<string, string>();
            EvalData[] types;
            int positive = 0;
            int negative = 0;
            using (Observable.Interval(TimeSpan.FromSeconds(30))
                             .Subscribe(item => log.Info(monitor)))
            {
                var reviews = ReadFiles();
                var subscriptionMessage = reviews.ToObservable()
                                                 .ObserveOn(TaskPoolScheduler.Default)
                                                 .Select(item => Observable.Start(() => ProcessReview(item), TaskPoolScheduler.Default))
                                                 .Merge()
                                                 .Merge()
                                                 .Where(item => item?.Stars != null)
                                                 .Where(item => item.Stars == 5 || item.Stars < 1.1);

                performance = new PrecisionRecallCalculator<bool>();
                types = subscriptionMessage.ToEnumerable().ToArray();
                positive = types.Count(item => item.CalculatedPositivity == PositivityType.Positive);
                negative = types.Count(item => item.CalculatedPositivity == PositivityType.Negative);
                var positiveOrg = types.Count(item => item.Original == PositivityType.Positive);
                var negativeOrg = types.Count(item => item.Original == PositivityType.Negative);
                log.Info($"Total (Positive): {positive}({positiveOrg}) Total (Negative): {negative}({negativeOrg})");
                if (BalancedTop.HasValue)
                {
                    var cutoff = positive > negative ? negative : positive;
                    cutoff = (int)(BalancedTop.Value * cutoff);
                    var negativeItems = types.OrderBy(item => item.Stars).Take(cutoff);
                    var positiveItems = types.OrderByDescending(item => item.Stars).Take(cutoff);
                    types = negativeItems.Union(positiveItems).OrderBy(item => Guid.NewGuid()).ToArray();
                    log.Info($"After balancing Total (Positive): {cutoff} Total (Negative): {cutoff}");
                }

                foreach (var item in types)
                {
                    if (item.Original.HasValue &&
                        item.Original.Value != PositivityType.Neutral)
                    {
                        performance.Add(item.Original.Value == PositivityType.Positive, item.CalculatedPositivity == PositivityType.Positive);
                    }
                }

                log.Info($"{performance.GetTotalAccuracy()} Precision (true): {performance.GetPrecision(true)} Precision (false): {performance.GetPrecision(false)}");
                SaveResult(types);
            }
        }

        protected virtual IEnumerable<EvalData> GetDataPacket(string file)
        {
            using (var streamRead = new StreamReader(file, Encoding.UTF8))
            {
                string line;
                while ((line = streamRead.ReadLine()) != null)
                {
                    long? id = null;
                    PositivityType? positivity = null;
                    var blocks = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (blocks.Length < 3)
                    {
                        log.Error($"Error: {line}");
                        yield break;
                    }

                    long idValue;
                    if (long.TryParse(blocks[0], out idValue))
                    {
                        id = idValue;
                    }

                    var textBlock = blocks[blocks.Length - 1];
                    var sentiment = blocks[blocks.Length - 2];
                    if (sentiment == "positive")
                    {
                        positivity = PositivityType.Positive;
                    }
                    else if (sentiment == "negative")
                    {
                        positivity = PositivityType.Negative;
                    }
                    else if (sentiment == "neutral")
                    {
                        positivity = PositivityType.Neutral;
                    }
                    else
                    {
                        int value;
                        if (int.TryParse(sentiment, out value))
                        {
                            positivity = value > 0 ? PositivityType.Positive : value < 0 ? PositivityType.Negative : PositivityType.Neutral;
                        }
                    }

                    if (textBlock[0] == '\"' &&
                        textBlock[textBlock.Length - 1] == '\"')
                    {
                        textBlock = textBlock.Substring(1, textBlock.Length - 2);
                    }

                    var text = cleanup.Cleanup(textBlock);
                    if (!exist.ContainsKey(text))
                    {
                        exist[text] = text;
                        yield return new EvalData(id.ToString(), positivity, text);
                    }
                }
            }
        }

        protected virtual void SaveResult(EvalData[] subscriptionMessage)
        {
            using (var streamWrite = new StreamWriter(Destination, false, Encoding.UTF8))
            {
                foreach (var item in subscriptionMessage)
                {
                    streamWrite.WriteLine($"{item.Id}\t{item.CalculatedPositivity.Value.ToString().ToLower()}\t{item.Text}");
                    streamWrite.Flush();
                }
            }
        }

        private void LoadBootstrap()
        {
            log.Info("Loading text splitter for bootstrapping");
            var config = new ConfigurationHandler();
            var splitterFactory = new SplitterFactory(new LocalCacheFactory(), config);
            bootStrapSplitter = splitterFactory.Create(POSTaggerType.SharpNLP);
            bootStrapSplitter.DataLoader.SentimentDataHolder.Clear();
            bootStrapSplitter.DataLoader.DisableFeatureSentiment = !UseInvert;
            var adjuster = new WeightSentimentAdjuster(bootStrapSplitter.DataLoader.SentimentDataHolder);
            adjuster.Adjust(Words);
        }

        private async Task<EvalData> ProcessReview(EvalData data)
        {
            await semaphore.WaitAsync()
                           .ConfigureAwait(false);
            try
            {
                var original = await bootStrapSplitter.Splitter.Process(new ParseRequest(data.Text)).ConfigureAwait(false);
                var bootReview = original.GetReview(bootStrapSplitter.DataLoader);

                var bootSentimentValue = bootReview.CalculateRawRating();
                var bootAllSentiments = bootReview.GetAllSentiments();

                if (bootSentimentValue.StarsRating.HasValue)
                {
                    if (bootAllSentiments.Length >= Minimum)
                    {
                        data.Stars = bootSentimentValue.StarsRating.Value;
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

        private IEnumerable<EvalData> ReadFiles()
        {
            foreach (var file in Directory.GetFiles(Path, "*.*", SearchOption.AllDirectories))
            {
                foreach (var semEvalData in GetDataPacket(file))
                {
                    monitor.ManualyCount();
                    yield return semEvalData;
                }
            }
        }
    }
}
