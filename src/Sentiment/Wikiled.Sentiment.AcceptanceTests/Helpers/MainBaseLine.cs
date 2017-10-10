using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NLog;
using NUnit.Framework;
using Wikiled.Sentiment.AcceptanceTests.Helpers.Data;
using Wikiled.Sentiment.Analysis.Amazon.Logic;
using Wikiled.Sentiment.Analysis.Processing;

namespace Wikiled.Sentiment.AcceptanceTests.Helpers
{
    public class MainBaseLine
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public TestRunner Training { get; private set; }

        public TestHelper Helper { get; private set; }

        private readonly string trainingLocation;

        private readonly string product;

        private readonly ProductCategory category;

        public MainBaseLine(string product, ProductCategory category)
        {
            this.category = category;
            this.product = product;
            trainingLocation = Path.Combine(TestContext.CurrentContext.TestDirectory, @"SVM", product);
        }

        public Task Train()
        {
            logger.Info("Trainning...");
            Helper = new TestHelper();
            Training = new TestRunner(
                Helper,
                new SentimentTestData(product)
                {
                    Category = category
                });

            logger.Info("Loading data...");
            TrainingClient trainingClient = new TrainingClient(Training.Active, Training.Load(), trainingLocation);
            logger.Info("Training...");
            return trainingClient.Train();
        }

        public async Task<TestingClient> Test(string testProduct, ProductCategory testCategory)
        {
            logger.Info("Testing...");
            TestRunner testing = new TestRunner(
                Helper,
                new SentimentTestData(testProduct)
                {
                    Category = testCategory
                });

            logger.Info("Loading data...");
            TestingClient testingClient = new TestingClient(testing.Active, testing.Load(), trainingLocation);
            testingClient.Init();
            await testingClient.Process().LastOrDefaultAsync();
            testingClient.Save(Path.Combine(trainingLocation, @"Result", testProduct));
            return testingClient;
        }
    }
}
