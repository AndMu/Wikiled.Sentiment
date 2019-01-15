using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Common.Logging;
using Wikiled.Common.Utilities.Resources;
using Wikiled.Console.Arguments;
using Wikiled.Console.HelperMethods;
using Wikiled.Sentiment.ConsoleApp.Analysis;
using Wikiled.Sentiment.ConsoleApp.Analysis.Config;
using Wikiled.Sentiment.Text.Resources;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Wikiled.Sentiment.ConsoleApp
{
    public class Program
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<Program>();

        public static async Task Main(string[] args)
        {
            NLog.LogManager.LoadConfiguration("nlog.config");
            var starter = new AutoStarter(ApplicationLogging.LoggerFactory, "Sentiment analysis", args);
            starter.LoggerFactory.AddNLog();
            starter.RegisterCommand<TestingCommand, TestingConfig>("test");
            starter.RegisterCommand<TrainCommand, TrainingConfig>("train");

            var configuration = new ConfigurationHandler();
            var resourcesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), configuration.GetConfiguration("Resources"));
            if (Directory.Exists(resourcesPath))
            {
                log.LogInformation("Resources folder {0} found.", resourcesPath);
            }
            else
            {
                var dataDownloader = new DataDownloader(ApplicationLogging.LoggerFactory);
                Task download = dataDownloader.DownloadFile(new Uri(configuration.GetConfiguration("dataset")), resourcesPath);
                await download.ConfigureAwait(false);
            }


#if NET472
            var fPreviousExecutionState = NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS | NativeMethods.ES_SYSTEM_REQUIRED);
            if (fPreviousExecutionState == 0)
            {
                log.LogError("SetThreadExecutionState failed.");
                return;
            }
#endif

            var source = new CancellationTokenSource();
            var task = starter.StartAsync(source.Token);
            System.Console.WriteLine("Please press enter to exit...");
            System.Console.ReadLine();
            if (!task.IsCompleted)
            {
                source.Cancel();
                source = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                await starter.StopAsync(source.Token).ConfigureAwait(false);
            }
        }
    }
}
