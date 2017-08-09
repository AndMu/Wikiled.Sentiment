using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CsvHelper;
using NLog;
using Wikiled.Arff.Persistence;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Resources;
using Wikiled.Redis.Config;
using Wikiled.Redis.Logic;
using Wikiled.Sentiment.Analysis.CrossDomain;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Cache;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.NLP.Stanford;
using Wikiled.Sentiment.Text.Parser.Cache;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.ConsoleApp.Machine
{
    /// <summary>
    /// trumpTest -Messages="c:\Out\messages.csv" -Out=c:\Out\result.xml -Weights=c:\Out\weights.csv
    /// </summary>
    public class TrumpTesting : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        [Required]
        public string Messages { get; set; }

        public string Weights { get; set; }

        public string Svm { get; set; }

        public string Features { get; set; }

        public bool FullWeightReset { get; set; }

        public int? Port { get; set; }

        [Required]
        public string Out { get; set; }

        public override void Execute()
        {
            log.Info("Testing Trump...");
            var splitterTask = Task.Run(() => LoadSplitter());
            var messagesTask = Task.Run(() => LoadMessages());
            Task.WaitAll(splitterTask, messagesTask);
            var splitter = splitterTask.Result;
            var data = messagesTask.Result;

            var reviews = splitter.Splitter.GetParsedReviewHolders(data);
            TestingClient client = new TestingClient(splitter, reviews);
            client.DisableSvm = true;
            client.DisableAspects = true;

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

            if (!string.IsNullOrEmpty(Svm))
            {
                log.Info($"Setting SVM: {Svm}");
                client.SvmPath = Svm;
                client.DisableSvm = false;
            }

            if (!string.IsNullOrEmpty(Features))
            {
                log.Info("Setting Features ON");
                client.DisableAspects = false;
                client.FeaturesPath = Features;
            }

            client.Init();
            var task = client.Process();
            task.Wait();
            client.Save(Out);
        }

        private ProcessingData LoadMessages()
        {
            log.Info($"Loading data {Messages}...");
            ProcessingData data = new ProcessingData();
            using (var stream = new StreamReader(Messages))
            {
                using (var csvData = new CsvReader(stream))
                {
                    while (csvData.Read())
                    {
                        var userId = csvData.GetField<long>("UserId");
                        var userName = csvData.GetField<string>("Name");
                        var messageId = csvData.GetField<long>("MessageId");
                        var latitude = csvData.GetField<string>("Latitude");
                        var longitude = csvData.GetField<string>("Longitude");
                        var text = csvData.GetField<string>("Text");
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            data.Add(
                                PositivityType.Positive,
                                new SingleProcessingData(
                                    new Document(text)
                                    {
                                        Id = messageId.ToString(),
                                        Author = userName,
                                        Stars = 1
                                    }));
                        }
                    }
                }
            }

            return data;
        }

        private SplitterHelper LoadSplitter()
        {
            log.Info("Initialize...");
            var redis = new RedisLink("Twitter", new RedisMultiplexer(new RedisConfiguration("localhost", Port ?? 6370)));
            redis.Open();
            var cacheFactory = new RedisDocumentCacheFactory(redis);
            var splitter = new SplitterHelper(
                cacheFactory,
                new ConfigurationHandler(),
                new StanfordFactory(cacheFactory));
            splitter.Load();
            return splitter;
        }
    }
}
