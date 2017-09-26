﻿using System;
using System.Linq;
using System.Reflection;
using NLog;
using Wikiled.Sentiment.ConsoleApp.Machine;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Helpers;

namespace Wikiled.Sentiment.ConsoleApp
{
    public class Program
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            log.Info("Starting {0} version utility...", Assembly.GetExecutingAssembly().GetName().Version);

            var fPreviousExecutionState =
                NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS | NativeMethods.ES_SYSTEM_REQUIRED);
            if (fPreviousExecutionState == 0)
            {
                log.Error("SetThreadExecutionState failed.");
                return;
            }

            if (args.Length == 0)
            {
                log.Warn("Please specify arguments");
                return;
            }
            try
            {
                Command command;
                if (string.Compare(args[0], "train", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new TrainingCommand();
                }
                else if (string.Compare(args[0], "semboot", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new SemEvalBoostrapCommand();
                }
                else if (string.Compare(args[0], "boot", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new SingleBoostrapCommand();
                }
                else if (string.Compare(args[0], "bootimdb", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new ImdbBoostrapCommand();
                }
                else if (string.Compare(args[0], "test", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new TestingCommand();
                }
                else if (string.Compare(args[0], "extract", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new TrainingCommand();
                }
                else if (string.Compare(args[0], "retrain", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new RetrainTrainingCommand();
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Root argument -" + args[0]);
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
