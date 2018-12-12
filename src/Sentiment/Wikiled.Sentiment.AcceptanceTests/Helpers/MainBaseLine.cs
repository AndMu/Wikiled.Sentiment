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

        private readonly string trainingLocation;

        public MainBaseLine(string product, ProductCategory category)
        {
            this.category = category;
            this.product = product;
            trainingLocation = Path.Combine(TestContext.CurrentContext.TestDirectory, @"SVM", product);
        }

        public TestRunner Training { get; private set; }

        public async Task<ITestingClient> Test(string testProduct, ProductCategory testCategory)
        {
            logger.LogInformation("Testing...");
            var testing = new TestRunner(TestHelper.Instance, new SentimentTestData(testProduct) { Category = testCategory });

            logger.LogInformation("Loading data...");
            var testingClient = testing.Active.GetTesting(trainingLocation);
            testingClient.TrackArff = true;
            testingClient.DisableAspects = true;
            testingClient.Init();
            await testingClient.Process(await GetData(testing.Load()).ConfigureAwait(false)).LastOrDefaultAsync();
            testingClient.Save(Path.Combine(trainingLocation, @"Result", testProduct));
            return testingClient;
        }

        public async Task Train()
        {
            logger.LogInformation("Trainning...");
            Training = new TestRunner(TestHelper.Instance, new SentimentTestData(product) { Category = category });
            logger.LogInformation("Loading data...");
            var trainingClient = Training.Active.GetTraining(trainingLocation);
            trainingClient.DisableAspects = true;
            logger.LogInformation("Training...");
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
