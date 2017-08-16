using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Logging;
using Wikiled.Core.Utility.Resources;
using Wikiled.Redis.Config;
using Wikiled.Redis.Logic;
using Wikiled.Sentiment.Analysis.Amazon.Logic;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Splitters;
using Wikiled.Sentiment.Text.Cache;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.NLP.Stanford;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.ConsoleApp.Machine
{
    public abstract class BaseRedis : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private AmazonRepository amazonRepository;

        private IRedisLink basicRedisManager;

        private ICachedDocumentsSource source;

        public abstract bool IsTraining { get; }

        [Required]
        public ProductCategory Category { get; set; }

        [Required]
        public int Port { get; set; }

        public string ProcessingPath { get; private set; }

        public string Product { get; set; }

        public string Server { get; set; }

        public string SvmPath { get; set; }

        public int? Year { get; set; }

        public int? Years { get; set; }

        protected ISplitterHelper Helper { get; set; }

        public override void Execute()
        {
            log.Info("Training Operation...");
            if (Year.HasValue)
            {
                log.Info("Year: {0} Years: {1}", Year, Years);
            }

            Server = Server ?? "localhost";
            using (basicRedisManager = new RedisLink("Wikiled", new RedisMultiplexer(new RedisConfiguration(Server, Port))))
            {
                basicRedisManager.Open();
                var redisDocumentFactory = new RedisDocumentCacheFactory(basicRedisManager);
                source = redisDocumentFactory.Create(POSTaggerType.Stanford);
                amazonRepository = new AmazonRepository(basicRedisManager);
                Helper = new SplitterFactory(redisDocumentFactory, new ConfigurationHandler()).Create(POSTaggerType.Stanford);
                log.Info("Loading libraries...");
                Helper.Load();
                ProcessingPath = string.IsNullOrEmpty(SvmPath) ? @".\Svm" : SvmPath;

                if (IsTraining)
                {
                    if (Year.HasValue)
                    {
                        ProcessingPath = Path.Combine(ProcessingPath, Year.ToString());
                    }

                    ProcessingPath = Path.Combine(ProcessingPath, Category.ToString());
                }

                log.Info("Processing path: {0}", ProcessingPath);
                Processing(LoadReviews());
            }
        }

        protected abstract void Processing(IObservable<IParsedDocumentHolder> reviews);

        private IObservable<IParsedDocumentHolder> LoadReviews()
        {
            IObservable<AmazonReview> reviews = null;
            if (!string.IsNullOrEmpty(Product))
            {
                log.Info($"Loading product: {Product}");
                if (Year.HasValue)
                {
                    log.Warn("<Year> parameter is ignored - selecting all reviews for given product");
                }

                reviews = amazonRepository.LoadProductReviews(Product);
            }
            else if (Year.HasValue)
            {
                int totalYears = Years ?? 1;
                for (int i = 0; i < totalYears; i++)
                {
                    int testingYear = Year.Value - i;
                    reviews = amazonRepository.LoadAll(testingYear, Category);
                }
            }
            else
            {
                log.Info("Selecting all documents....");
                reviews = amazonRepository.LoadAll(Category);
            }

            var monitor = new PerformanceMonitor(1000);
            return reviews.Select(review => Select(monitor, review)).Merge();
        }

        private async Task<IParsedDocumentHolder> Select(PerformanceMonitor monitor, AmazonReview review)
        {
            var parsed = await source.GetById(review.Id).ConfigureAwait(false);
            monitor.Increment();
            return new ParsedDocumentHolder(
                review.CreateDocument(),
                parsed);
        }
    }
}
