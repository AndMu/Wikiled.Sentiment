using System;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Extensions;
using Wikiled.Core.Utility.Resources;
using Wikiled.Redis.Config;
using Wikiled.Redis.Logic;
using Wikiled.Sentiment.Analysis.Amazon;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Cache;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Text.Analysis.POS;
using Wikiled.Sentiment.Text.NLP.Stanford;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Cache;

namespace Wikiled.Sentiment.ConsoleApp.Unpacking
{
    /// <summary>
    ///     lemma -Out=E:\Data\out_Video_psenti.txt -Port=6370 -Category=Video
    /// </summary>
    public class AmazonLemmaExtract : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private ICachedDocumentsSource cache;

        private SplitterHelper helper;

        private StreamWriter stream;

        private int total;

        [Required]
        public ProductCategory Category { get; set; }

        [Required]
        public string Out { get; set; }

        [Required]
        public int Port { get; set; }

        public override void Execute()
        {
            log.Info("Processing: {0}", Category);
            ProcessRedis().Wait();
        }

        private static string GetText(IWordItem wordItem)
        {
            if (string.IsNullOrEmpty(wordItem.Text))
            {
                return string.Empty;
            }

            var text = wordItem.Text.ToLower();

            if (!text.HasLetters() ||
                text.EndsWith(".com"))
            {
                return wordItem.Text;
            }

            var words = text.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
            if (wordItem.IsInvertor)
            {
                return !wordItem.IsAttached() ? "_NOT_" : string.Empty;
            }

            text = string.Empty;
            for (int i = 0; i < words.Length; i++)
            {
                if (i == 0 &&
                    wordItem.Relationship.Inverted != null)
                {
                    return "NOT_" + words[i];
                }

                if (i > 0 &&
                    text.Length > 0)
                {
                    text += " ";
                }

                text += words[i];
            }

            return text;
        }

        private async Task ProcessDocument(AmazonReview amazonReviewId)
        {
            var doc = await cache.GetById(amazonReviewId.Id);
            if (doc != null)
            {
                var construct = new ParsedReviewFactory(helper.DataLoader, doc);
                var review = construct.Create();
                StringBuilder builder = new StringBuilder();
                foreach (var item in review.Items)
                {
                    var text = GetText(item);
                    if (builder.Length > 0)
                    {
                        builder.Append(" ");
                    }

                    builder.Append(text);
                }

                lock (stream)
                {
                    stream.WriteLine(builder.ToString());
                }
            }

            if (Interlocked.Increment(ref total)%1000 == 0)
            {
                log.Info("Procesed: {0}", total);
            }
        }

        private async Task ProcessRedis()
        {
            using (
                RedisLink manager = new RedisLink("Wikiled",
                    new RedisMultiplexer(new RedisConfiguration("localhost", Port))))
            {
                manager.Open();
                var cacheFactory = new RedisDocumentCacheFactory(manager);
                helper = new SplitterHelper(cacheFactory, new ConfigurationHandler(), new StanfordFactory(cacheFactory));
                cache = cacheFactory.Create(POSTaggerType.Stanford);
                helper.Load();
                AmazonRepository repository = new AmazonRepository(manager);
                var documents = repository.LoadAll(Category);

                var size = 1024*1024;

                int totalParallelisation = Environment.ProcessorCount;
                TaskFactory taskFactory = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(totalParallelisation));
                var scheduler = new TaskPoolScheduler(taskFactory);
                SemaphoreSlim startSemaphore = new SemaphoreSlim(totalParallelisation, totalParallelisation);
                using (stream = new StreamWriter(Out, false, Encoding.ASCII, size))
                {
                    stream.AutoFlush = false;
                    var selector = documents
                        .ObserveOn(scheduler)
                        .Select(review => ProcessReview(review, startSemaphore))
                        .Merge();
                    await selector;
                }

                stream.Flush();
                log.Info("Completed...");
            }
        }

        private async Task<AmazonReview> ProcessReview(AmazonReview review, SemaphoreSlim startSemaphore)
        {
            try
            {
                await startSemaphore.WaitAsync();
                await ProcessDocument(review);
                return review;
            }
            finally
            {
                startSemaphore.Release(1);
            }
        }
    }
}

