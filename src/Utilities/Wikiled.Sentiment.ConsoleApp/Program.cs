using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Helpers;
using Wikiled.Sentiment.ConsoleApp.Analysis;
using Wikiled.Sentiment.ConsoleApp.Extraction;
using Wikiled.Sentiment.ConsoleApp.Extraction.Bootstrap;

namespace Wikiled.Sentiment.ConsoleApp
{
    public class Program
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            log.Info("Starting {0} version utility...", Assembly.GetExecutingAssembly().GetName().Version);
            List<Command> commandsList = new List<Command>();
            commandsList.Add(new TrainCommand());
            commandsList.Add(new SemEvalBoostrapCommand());
            commandsList.Add(new SingleBoostrapCommand());
            commandsList.Add(new ImdbBoostrapCommand());
            commandsList.Add(new TestingCommand());
            commandsList.Add(new RetrainCommand());
            commandsList.Add(new ExtractAttributesCommand());
            var commands = commandsList.ToDictionary(item => item.Name, item => item, StringComparer.OrdinalIgnoreCase);

            var fPreviousExecutionState =
                NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS | NativeMethods.ES_SYSTEM_REQUIRED);
            if (fPreviousExecutionState == 0)
            {
                log.Error("SetThreadExecutionState failed.");
                return;
            }
          
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
                command.Execute();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                Console.ReadLine();
            }
        }
    }
}
