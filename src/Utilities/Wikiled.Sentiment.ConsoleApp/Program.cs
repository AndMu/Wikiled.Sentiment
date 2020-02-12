using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Wikiled.Common.Logging;
using Wikiled.Common.Utilities.Resources;
using Wikiled.Console.Arguments;
using Wikiled.Sentiment.ConsoleApp.Analysis;
using Wikiled.Sentiment.ConsoleApp.Analysis.Config;
using Wikiled.Sentiment.Text.Config;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Wikiled.Sentiment.ConsoleApp
{
    public class Program
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<Program>();

        private static Task task;

        private static CancellationTokenSource source;

        private static AutoStarter starter;

        public static async Task Main(string[] args)
        {
            NLog.LogManager.LoadConfiguration("nlog.config");
            starter = new AutoStarter(ApplicationLogging.LoggerFactory, "Sentiment analysis", args);
            starter.Collection.AddLogging(item => item.AddNLog());
            starter.RegisterCommand<TestingCommand, TestingConfig>("test");
            starter.RegisterCommand<TrainCommand, TrainingConfig>("train");
            starter.RegisterCommand<BoostrapCommand, BootsrapConfig>("boot");

            var config = LexiconConfigExtension.Load();
            if (Directory.Exists(config.Resources))
            {
                log.LogInformation("Resources folder {0} found.", config.Resources);
            }
            else
            {
                var dataDownloader = new DataDownloader(ApplicationLogging.LoggerFactory);
                Task download = dataDownloader.DownloadFile(new Uri(config.Remote), config.Lexicon);
                await download.ConfigureAwait(false);
            }

            try
            {
                source = new CancellationTokenSource();
                task = starter.StartAsync(source.Token);
                System.Console.WriteLine("Please press CTRL+C to break...");
                System.Console.CancelKeyPress += ConsoleOnCancelKeyPress;
                await starter.Status.LastOrDefaultAsync();
                System.Console.WriteLine("Exiting...");
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
        }

        private static async void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (!task.IsCompleted)
            {
                source.Cancel();
            }

            source = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await starter.StopAsync(source.Token).ConfigureAwait(false);
        }
    }
}
