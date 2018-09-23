using System;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MoreLinq;
using NLog;
using NUnit.Framework;
using Wikiled.Amazon.Logic;
using Wikiled.Sentiment.AcceptanceTests.Helpers.Data;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Pipeline;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.AcceptanceTests.Helpers
{
    public class MainBaseLine
    {
        private readonly ProductCategory category;

        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly string product;

        private readonly string trainingLocation;

        public MainBaseLine(string product, ProductCategory category)
        {
            this.category = category;
            this.product = product;
            trainingLocation = Path.Combine(TestContext.CurrentContext.TestDirectory, @"SVM", product);
        }

        public TestRunner Training { get; private set; }

        public async Task<TestingClient> Test(string testProduct, ProductCategory testCategory)
        {
            logger.Info("Testing...");
            TestRunner testing = new TestRunner(TestHelper.Instance, new SentimentTestData(testProduct) { Category = testCategory });

            logger.Info("Loading data...");
            ProcessingPipeline pipeline = new ProcessingPipeline(TaskPoolScheduler.Default, testing.Active);
            TestingClient testingClient = new TestingClient(pipeline, trainingLocation);
            testingClient.TrackArff = true;
            testingClient.DisableAspects = true;
            testingClient.Init();
            await testingClient.Process(await GetData(testing.Load()).ConfigureAwait(false)).LastOrDefaultAsync();
            testingClient.Save(Path.Combine(trainingLocation, @"Result", testProduct));
            return testingClient;
        }

        public async Task Train()
        {
            logger.Info("Trainning...");
            Training = new TestRunner(TestHelper.Instance, new SentimentTestData(product) { Category = category });

            logger.Info("Loading data...");
            ProcessingPipeline pipeline = new ProcessingPipeline(TaskPoolScheduler.Default, Training.Active);
            TrainingClient trainingClient = new TrainingClient(pipeline, trainingLocation);
            trainingClient.DisableAspects = true;
            logger.Info("Training...");
            await trainingClient.Train(await GetData(Training.Load()).ConfigureAwait(false)).ConfigureAwait(false);
        }

        private async Task<IObservable<IParsedDocumentHolder>> GetData(IObservable<IParsedDocumentHolder> source)
        {
            var data = await source.ToArray();
            var sorted = data.OrderBy(item => item.Original.Id, OrderByDirection.Ascending);
            return sorted.ToObservable();
        }
    }
}
