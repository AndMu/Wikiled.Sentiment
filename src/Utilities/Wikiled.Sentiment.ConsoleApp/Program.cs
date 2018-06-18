using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Wikiled.Console.Arguments;
using Wikiled.Sentiment.ConsoleApp.Analysis;
using Wikiled.Sentiment.ConsoleApp.Extraction;
using Wikiled.Sentiment.ConsoleApp.Extraction.Bootstrap;
using Wikiled.Sentiment.Text.Resources;

namespace Wikiled.Sentiment.ConsoleApp
{
    public class Program
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static async Task Main(string[] args)
        {
            log.Info("Starting {0} version utility...", Assembly.GetExecutingAssembly().GetName().Version);
            ConfigurationHandler configuration = new ConfigurationHandler();
            var resourcesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), configuration.GetConfiguration("Resources"));
            if (Directory.Exists(resourcesPath))
            {
                log.Info("Resources folder {0} found.", resourcesPath);
            }
            else
            {
                DataDownloader dataDownloader = new DataDownloader();
                var task = dataDownloader.DownloadFile(new Uri(configuration.GetConfiguration("dataset")), resourcesPath);
                task.Wait();
            }

            List<Command> commandsList = new List<Command>();
            commandsList.Add(new TrainCommand());
            commandsList.Add(new SemEvalBoostrapCommand());
            commandsList.Add(new SingleBoostrapCommand());
            commandsList.Add(new ImdbBoostrapCommand());
            commandsList.Add(new TestingCommand());
            commandsList.Add(new ExtractAttributesCommand());
            var commands = commandsList.ToDictionary(item => item.Name, item => item, StringComparer.OrdinalIgnoreCase);

            #if NET462
                var fPreviousExecutionState = NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS | NativeMethods.ES_SYSTEM_REQUIRED);
                if (fPreviousExecutionState == 0)
                {
                    log.Error("SetThreadExecutionState failed.");
                    return;
                }
            #endif

            try
            {
                if (args.Length == 0)
                {
                    log.Warn("Please specify arguments");
                    CommandLineParser.PrintCommands(commands.Values);
                    return;
                }

                if (!commands.TryGetValue(args[0], out var command))
                {
                    log.Error("Unknown Command");
                    return;
                }

                command.ParseArguments(args.Skip(1)); // or CommandLineParser.ParseArguments(c, args.Skip(1))
                await command.StartExecution(CancellationToken.None);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                System.Console.ReadLine();
            }
        }
    }
}
