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
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.Twitter;

namespace Wikiled.Sentiment.ConsoleApp.Machine
{
    /// <summary>
    /// boot -Words=words.csv -Path="E:\DataSets\SemEval\All\out\ -Destination=c:\DataSets\SemEval\train.txt
    /// </summary>
    public class BoostrapCommand : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private ISplitterHelper bootStrapSplitter;

        private ISplitterHelper defaultSplitter;

        private PerformanceMonitor monitor;

        [Required]
        public string Destination { get; set; }

        [Required]
        public string Path { get; set; }

        [Required]
        public string Words { get; set; }

        private PrecisionRecallCalculator<bool> performance;

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(Environment.ProcessorCount / 2);

        public override void Execute()
        {
            LoadDefault();
            LoadBootstrap();
            monitor = new PerformanceMonitor(0);
            List<PositivityType?> types = new List<PositivityType?>();
            using (Observable.Interval(TimeSpan.FromSeconds(30)).Subscribe(item => log.Info(monitor)))
            {
                var reviews = ReadFiles();
                var subscriptioMessage = reviews.ToObservable()
                                                .ObserveOn(TaskPoolScheduler.Default)
                                                .Select(item => Observable.Start(() => ProcessReview(item), TaskPoolScheduler.Default))
                                                .Merge()
                                                .Merge()
                                                .Where(item => item.Text != null && (item.Stars == 5 || item.Stars < 3));

                performance = new PrecisionRecallCalculator<bool>();
                var groups = subscriptioMessage.ToEnumerable().GroupBy(GetPositivityType).ToArray();
                var result = groups.SelectMany(item => item);
                using (var streamWrite = new StreamWriter(Destination, false, Encoding.UTF8))
                {
                    foreach (var item in result)
                    {
                        var calculated = GetPositivityType(item);
                        types.Add(calculated);
                        if (calculated.HasValue)
                        {
                            if (item.Original.HasValue)
                            {
                                performance.Add(item.Original.Value == PositivityType.Positive, calculated == PositivityType.Positive);
                            }

                            streamWrite.WriteLine($"{item.Id}\t{calculated.Value.ToString().ToLower()}\t{item.Text}");
                            streamWrite.Flush();
                        }
                    }
                }
            }

            log.Info($"Total (Positive): {types.Count(item => item == PositivityType.Positive)} Total (Negative): {types.Count(item => item == PositivityType.Negative)} Total (Neutral): {types.Count(item => item == PositivityType.Neutral)}");
            log.Info($"{performance.GetTotalAccuracy()} Precision (true): {performance.GetPrecision(true)} Precision (false): {performance.GetPrecision(false)}");
        }

        private static PositivityType? GetPositivityType(ValueTuple<long?, double?, PositivityType?, string> item)
        {
            PositivityType? calculated;
            if (item.Item2 > 3)
            {
                calculated = PositivityType.Positive;
            }
            else
            {
                calculated = PositivityType.Negative;
            }

            return calculated;
        }

        private void LoadBootstrap()
        {
            log.Info("Loading text splitter for bootstrapping");
            var config = new ConfigurationHandler();
            var splitterFactory = new SplitterFactory(new LocalCacheFactory(), config);
            bootStrapSplitter = splitterFactory.Create(POSTaggerType.SharpNLP);
            bootStrapSplitter.DataLoader.SentimentDataHolder.Clear();
            bootStrapSplitter.DataLoader.DisableFeatureSentiment = true;
            var adjuster = new WeightSentimentAdjuster(bootStrapSplitter.DataLoader.SentimentDataHolder);
            adjuster.Adjust(Words);
        }

        private void LoadDefault()
        {
            log.Info("Loading default text splitter");
            var config = new ConfigurationHandler();
            var splitterFactory = new SplitterFactory(new LocalCacheFactory(), config);
            defaultSplitter = splitterFactory.Create(POSTaggerType.SharpNLP);
        }

        private async Task<(long? Id, double? Stars, PositivityType? Original, string Text)> ProcessReview((long? Id, PositivityType? Positivity, string Text) data)
        {
            await semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                var main = defaultSplitter.Splitter.Process(new ParseRequest(data.Text));
                var original = bootStrapSplitter.Splitter.Process(new ParseRequest(data.Text));
                await Task.WhenAll(main, original).ConfigureAwait(false);

                var mainResult = main.Result;
                var originalReview = mainResult.GetReview(defaultSplitter.DataLoader);
                var bootReview = original.Result.GetReview(bootStrapSplitter.DataLoader);

                var bootSentimentValue = bootReview.CalculateRawRating();
                var bootAllSentiments = bootReview.GetAllSentiments();

                var originalSentimentValue = originalReview.CalculateRawRating();

                (long? Id, double? Stars, PositivityType? Original, string Text) nullData = (null, null, null, null);


                if (bootSentimentValue.StarsRating.HasValue)
                {
                    if (originalSentimentValue.StarsRating.HasValue &&
                        originalSentimentValue.IsPositive != bootSentimentValue.IsPositive)
                    {
                        // disagreement between lexicons
                        return nullData;
                    }

                    if (bootAllSentiments.Length > 2)
                    {
                        return (data.Id, bootSentimentValue.StarsRating.Value, data.Positivity, data.Text);
                    }
                }

                return nullData;

            }
            finally
            {
                monitor.Increment();
                semaphore.Release();
            }
        }
        private IEnumerable<(long? Id, PositivityType? Positivity, string Text)> ReadFiles()
        {
            MessageCleanup cleanup = new MessageCleanup();
            Dictionary<string, string> exist = new Dictionary<string, string>();
            foreach (var file in Directory.GetFiles(Path, "*.*", SearchOption.AllDirectories))
            {
                using (var streamRead = new StreamReader(file, Encoding.UTF8))
                {
                    string line = string.Empty;
                    while ((line = streamRead.ReadLine()) != null)
                    {
                        long? id = null;
                        PositivityType? positivity = null;
                        var blocks = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (blocks.Length < 3)
                        {
                            log.Error($"Error: {line}");
                            continue;
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
                            monitor.ManualyCount();
                            yield return (id, positivity, text);
                        }
                    }
                }
            }
        }
    }
}
