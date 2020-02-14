using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Common.Logging;
using Wikiled.Console.Arguments;
using Wikiled.Sentiment.ConsoleApp.Analysis;
using Wikiled.Sentiment.ConsoleApp.Analysis.Config;
using Wikiled.Sentiment.Text.Config;

namespace Wikiled.Sentiment.ConsoleApp
{
    public class Program
    {
        private static Task task;

        private static CancellationTokenSource source;

        private static AutoStarter starter;

        public static async Task Main(string[] args)
        {
            NLog.LogManager.LoadConfiguration("nlog.config");
            starter = new AutoStarter(ApplicationLogging.LoggerFactory, "Sentiment analysis", args);
            starter.Collection.AddLogging(loggingBuilder =>
            {
                // configure Logging with NLog
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.AddNLog();
            });

            starter.RegisterCommand<TestingCommand, TestingConfig>("test");
            starter.RegisterCommand<TrainCommand, TrainingConfig>("train");
            starter.RegisterCommand<BoostrapCommand, BootsrapConfig>("boot");

            starter.Init = async provider =>
            {
                var loader = provider.GetRequiredService<LexiconConfigLoader>();
                await loader.Download().ConfigureAwait(false);
            };

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
