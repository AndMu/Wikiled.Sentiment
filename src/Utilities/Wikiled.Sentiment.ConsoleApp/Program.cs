using System;
using System.Linq;
using System.Reflection;
using NLog;
using Wikiled.Sentiment.ConsoleApp.Machine;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Helpers;
using Wikiled.Sentiment.ConsoleApp.Machine.Parser;
using Wikiled.Sentiment.ConsoleApp.Unpacking;

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
                if (string.Compare(args[0], "training", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new TrainingCommand();
                }
                else if (string.Compare(args[0], "raw", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new RawTrainingCommand();
                }
                else if (string.Compare(args[0], "lemma", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new AmazonLemmaExtract();
                }
                else if (string.Compare(args[0], "testing", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new TestingCommand();
                }
                else if (string.Compare(args[0], "rawTest", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new RawTestingCommand();
                }
                else if (string.Compare(args[0], "tokenize", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new SentimentTokenizeCommand();
                }
                else if (string.Compare(args[0], "extract", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new TrainingCommand();
                }
                else if (string.Compare(args[0], "trumpTest", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new TrumpTesting();
                }
                else if (string.Compare(args[0], "retrain", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new RetrainTrainingCommand();
                }
                else if (string.Compare(args[0], "amazon", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new AmazonParser();
                }
                else if (string.Compare(args[0], "amazonredis", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new AmazonParserRedis();
                }
                else if (string.Compare(args[0], "amazontext", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new AmazonTextExtract();
                }
                else if (string.Compare(args[0], "amazontextex", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new AmazonTextExtractClean();
                }
                else if (string.Compare(args[0], "amazonProduct", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new AmazonProductExtract();
                }
                else if (string.Compare(args[0], "vector", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new ContextVectorUnpackingCommand();
                }
                else if (string.Compare(args[0], "unpackcsv", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new CsvUnpackingCommand();
                }
                else if (string.Compare(args[0], "arffsort", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new ArffSort();
                }
                else if (string.Compare(args[0], "merge", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    command = new ArffMerge();
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
