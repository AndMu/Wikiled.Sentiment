using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Common.Extensions;
using Wikiled.Sentiment.Analysis.Containers;
using Wikiled.Sentiment.Analysis.Pipeline.Persistency;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Persistency;
using Wikiled.Sentiment.ConsoleApp.Analysis.Config;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.ConsoleApp.Analysis
{
    /// <summary>
    ///     test [-Articles="C:\Cloud\OneDrive\Study\Medical\articles.xml"] [-Model=.\Svm] -Out=.\results
    /// </summary>
    [Description("pSenti testing")]
    public class TestingCommand : BaseRawCommand<TestingConfig>
    {
        private readonly IPipelinePersistency persistency;

        public TestingCommand(ILogger<TestingCommand> log, TestingConfig config, IDataLoader loader, ISessionContainer container, IPipelinePersistency persistency)
            : base(log, config, loader, container)
        {
            this.persistency = persistency;
            Semaphore = new SemaphoreSlim(3000);
        }

        protected override async Task Process(IObservable<IParsedDocumentHolder> reviews, ISessionContainer container, ISentimentDataHolder sentimentAdjustment)
        {
            ITestingClient client;
            Config.Out.EnsureDirectoryExistence();
            using (persistency)
            {
                persistency.Start(Config.Out);
                persistency.Debug = Config.Debug;
                persistency.ExtractStyle = Config.ExtractStyle;
                client = container.GetTesting(Config.Model);
                container.Context.Lexicon = sentimentAdjustment;
                using (Observable.Interval(TimeSpan.FromSeconds(30))
                    .Subscribe(item => Logger.LogInformation(client.Pipeline.Monitor.ToString())))
                {
                    client.TrackArff = Config.TrackArff;
                    client.UseBagOfWords = Config.UseBagOfWords;
                    client.Init();
                    await client.Process(reviews.ObserveOn(TaskPoolScheduler.Default))
                        .Select(
                            item =>
                            {
                                persistency.Save(item);
                                Semaphore.Release();
                                return item;
                            })
                        .LastOrDefaultAsync();
                }

                if (!Config.TrackArff)
                {
                    client.Save(Config.Out);
                }
            }

            Logger.LogInformation($"Testing performance {client.GetPerformanceDescription()}");
        }
    }
}