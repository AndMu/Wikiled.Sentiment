using NLog;
using Wikiled.Arff.Persistence;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.MachineLearning;

namespace Wikiled.Sentiment.ConsoleApp.Machine
{
    public class ExtractSentiment : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public string SvmPath { get; set; }

        public override void Execute()
        {
            log.Info("Starting sentiment extraction.");
            log.Info("Loading {0}", SvmPath);
            var machine = MachineSentiment<PositivityType>.Load(SvmPath);
        }
    }
}
