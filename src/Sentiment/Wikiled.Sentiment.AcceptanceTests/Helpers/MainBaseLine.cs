using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MoreLinq;
using NUnit.Framework;
using Wikiled.Amazon.Logic;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.AcceptanceTests.Helpers.Data;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.AcceptanceTests.Helpers
{
    public class MainBaseLine
    {
        private readonly ProductCategory category;

        private readonly ILogger logger = ApplicationLogging.CreateLogger<MainBaseLine>();

        private readonly string product;

        private readonly string resultPath;

        public MainBaseLine(string product, ProductCategory category)
        {
            this.category = category;
            this.product = product;
            resultPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Results", product);
        }

        public TestRunner Training { get; private set; }

        public async Task<ITestingClient> Test(string testProduct, ProductCategory testCategory)
        {
            logger.LogInformation("Testing...");
            var testing = new TestRunner(TestHelper.Instance, new SentimentTestData(testProduct) { Category = testCategory });

            logger.LogInformation("Loading data...");
            var testingClient = testing.Active.GetTesting(Path.Combine(resultPath, "Training"));
            testingClient.TrackArff = true;
            testingClient.DisableAspects = true;
            testingClient.Init();
            await testingClient.Process(await GetData(testing.Load()).ConfigureAwait(false)).LastOrDefaultAsync();
            testingClient.Save(Path.Combine(resultPath, @"Tests", testProduct));
            return testingClient;
        }

        public async Task Train()
        {
            logger.LogInformation("Initializing Training...");
            Training = new TestRunner(TestHelper.Instance, new SentimentTestData(product) { Category = category });
            logger.LogInformation("Loading data...");
            var trainingClient = Training.Active.GetTraining(Path.Combine(resultPath, "Training"));
            trainingClient.DisableAspects = true;
            logger.LogInformation("Training...");
            await trainingClient.Train(await GetData(Training.Load()).ConfigureAwait(false)).ConfigureAwait(false);
        }

        private async Task<IObservable<IParsedDocumentHolder>> GetData(IObservable<IParsedDocumentHolder> source)
        {
            var data = await source.ToArray();
            var sorted = data.OrderBy(item => item.GetOriginal().Result.Id, OrderByDirection.Ascending);
            return sorted.ToObservable();
        }
    }
}
