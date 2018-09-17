using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NLog;
using NUnit.Framework;
using Wikiled.Amazon.Logic;
using Wikiled.Sentiment.AcceptanceTests.Helpers.Data;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Pipeline;

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

        public TestHelper Helper { get; private set; }

        public TestRunner Training { get; private set; }

        public async Task<TestingClient> Test(string testProduct, ProductCategory testCategory)
        {
            logger.Info("Testing...");
            TestRunner testing = new TestRunner(Helper, new SentimentTestData(testProduct) { Category = testCategory });

            logger.Info("Loading data...");
            ProcessingPipeline pipeline = new ProcessingPipeline(TaskPoolScheduler.Default, testing.Active);
            TestingClient testingClient = new TestingClient(pipeline, trainingLocation);
            testingClient.TrackArff = true;
            testingClient.Init();
            await testingClient.Process(testing.Load()).LastOrDefaultAsync();
            testingClient.Save(Path.Combine(trainingLocation, @"Result", testProduct));
            return testingClient;
        }

        public Task Train()
        {
            logger.Info("Trainning...");
            Helper = new TestHelper();
            Training = new TestRunner(Helper, new SentimentTestData(product) { Category = category });

            logger.Info("Loading data...");
            ProcessingPipeline pipeline = new ProcessingPipeline(TaskPoolScheduler.Default, Training.Active);
            TrainingClient trainingClient = new TrainingClient(pipeline, trainingLocation);
            logger.Info("Training...");
            return trainingClient.Train(Training.Load());
        }
    }
}
