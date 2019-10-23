using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Threading.Tasks;
using Wikiled.Common.Utilities.Modules;
using Wikiled.Sentiment.Analysis.Containers;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.NLP.Repair;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.AcceptanceTests.Containers
{
    [TestFixture]
    public class ContainersTests
    {
        [Test]
        public async Task Construct()
        {
            var configuration = new ConfigurationHandler();
            configuration.SetConfiguration("lexicons", "lexicons");
            configuration.StartingLocation = TestContext.CurrentContext.TestDirectory;

            var builder = new ServiceCollection();
            builder.RegisterModule<LoggingModule>();
            builder.RegisterModule<CommonModule>();
            builder.RegisterModule(new SentimentMainModule());
            builder.RegisterModule(new SentimentServiceModule(configuration) { Lexicons = "." });
            var container = builder.BuildServiceProvider();

            for (int i = 0; i < 2; i++)
            {
                using var scope = container.CreateScope();
                var session = scope.ServiceProvider.GetService<ISessionContainer>();
                var client = session.GetTesting();
                client.Init();

                var result = await client
                                   .Process(new ParsingDocumentHolder(session.GetTextSplitter(),
                                                                      session.GetWordFactory(),
                                                                      session.Resolve<IContextSentenceRepairHandler>(),
                                                                      new Document("I like beer")))
                                   .ConfigureAwait(false);
                Assert.AreEqual(5, result.Adjustment.Rating.StarsRating);
            }
        }
    }
}
