using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Common.Logging;
using Wikiled.Common.Utilities.Resources;
using Wikiled.Console.Arguments;
using Wikiled.Console.HelperMethods;
using Wikiled.Sentiment.ConsoleApp.Analysis;
using Wikiled.Sentiment.ConsoleApp.Extraction;
using Wikiled.Sentiment.ConsoleApp.Extraction.Bootstrap;
using Wikiled.Sentiment.Text.Resources;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Wikiled.Sentiment.ConsoleApp
{
    public class Program
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<Program>();

        public static async Task Main(string[] args)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            log.LogInformation("Starting {0} version utility...", Assembly.GetExecutingAssembly().GetName().Version);
            var configuration = new ConfigurationHandler();
            var resourcesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), configuration.GetConfiguration("Resources"));
            if (Directory.Exists(resourcesPath))
            {
                log.LogInformation("Resources folder {0} found.", resourcesPath);
            }
            else
            {
                var dataDownloader = new DataDownloader(loggerFactory);
                Task task = dataDownloader.DownloadFile(new Uri(configuration.GetConfiguration("dataset")), resourcesPath);
                task.Wait();
            }

            var commandsList = new List<Command>
            {
                new TrainCommand(),
                new SemEvalBoostrapCommand(),
                new SingleBoostrapCommand(),
                new ImdbBoostrapCommand(),
                new TestingCommand(),
                new ExtractAttributesCommand()
            };

            var commands = commandsList.ToDictionary(item => item.Name, item => item, StringComparer.OrdinalIgnoreCase);

#if NET472
            var fPreviousExecutionState = NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS | NativeMethods.ES_SYSTEM_REQUIRED);
            if (fPreviousExecutionState == 0)
            {
                 log.LogError("SetThreadExecutionState failed.");
                return;
            }
#endif

            try
            {
                if (args.Length == 0)
                {
                    log.LogWarning("Please specify arguments");
                    CommandLineParser.PrintCommands(commands.Values);
                    return;
                }

                if (!commands.TryGetValue(args[0], out Command command))
                {
                    log.LogError("Unknown Command");
                    return;
                }

                command.ParseArguments(args.Skip(1)); // or CommandLineParser.ParseArguments(c, args.Skip(1))
                await command.StartExecution(CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error");
                System.Console.ReadLine();
            }
        }
    }
}
