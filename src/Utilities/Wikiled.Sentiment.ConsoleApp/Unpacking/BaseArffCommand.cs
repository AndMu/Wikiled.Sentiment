using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Wikiled.Arff.Persistence;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.Sentiment.ConsoleApp.Unpacking
{
    public abstract class BaseArffCommand: Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private int current;

        private int total;

        [Required]
        public string Source { get; set; }

        public string Out { get; set; }

        public override void Execute()
        {
            log.Info("Starting...");
            if (string.IsNullOrEmpty(Out))
            {
                Out = Source;
            }

            var files = Directory.GetFiles(Source, "*.arff").ToArray();
            total = files.Length;
            log.Info("Processing {0} files...", total);
            current = 0;

            List<Task> tasks = files.Select(item => Task.Run(() => ProcessSingle(item))).ToList();
            Task.WhenAll(tasks).Wait();
        }

        protected abstract void Process(string file, IArffDataSet arff);

        private void ProcessSingle(string file)
        {
            Interlocked.Increment(ref current);
            log.Debug("Processing {0} {1}/{2}", Path.GetFileName(file), current, total);
            var arff = ArffDataSet.Load<PositivityType>(file);
            Process(file, arff);
        }
    }
}
