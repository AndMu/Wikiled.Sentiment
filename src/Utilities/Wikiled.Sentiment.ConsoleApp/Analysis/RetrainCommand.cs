using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading;
using NLog;
using Wikiled.Arff.Normalization;
using Wikiled.Arff.Persistence;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.MachineLearning;

namespace Wikiled.Sentiment.ConsoleApp.Analysis
{
    /// <summary>
    /// retrain -Trained=.\SvmTwo
    /// </summary>
    [Description("pSenti retrain command")]
    public class RetrainCommand : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        [Required]
        public string SvmPath { get; set; }

        public override void Execute()
        {
            log.Info("Re-train Operation...");
            var arffOldPath = Path.Combine(SvmPath, @"data_old.arff");
            var machine = MachineSentiment<PositivityType>.Load(SvmPath);
            machine.Arff.Save(arffOldPath);
            machine.Header.Normalization = NormalizationType.L2;
            machine.Arff.Normalize(NormalizationType.L2);
            machine = machine.Retrain(CancellationToken.None).Result;
            machine.Save(SvmPath);
        }
    }
}
