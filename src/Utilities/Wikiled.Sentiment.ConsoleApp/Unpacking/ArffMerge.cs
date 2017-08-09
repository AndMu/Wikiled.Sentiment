using System.IO;
using NLog;
using Wikiled.Arff.Persistence;

namespace Wikiled.Sentiment.ConsoleApp.Unpacking
{
    // merge -Source e:\results -Out -Source e:\results
    public class ArffMerge : BaseArffCommand
    {
        private IArffDataSet result;

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        protected override void Process(string file, IArffDataSet arff)
        {
            foreach (var review in arff.Documents)
            {
                var copy = result.AddDocument();
                foreach (var word in review.GetRecords())
                {
                    copy.SetRecord(word);
                }
            }
        }

        public override void Execute()
        {
            result = ArffDataSet.Create<PositivityType>("Aggregated");
            base.Execute();

            if (Out.IndexOf(".arff") < 0)
            {
                Out = Path.Combine(Out, "aggregated.arff");
            }

            log.Info("Saving to {0}...", Out);
            result.Save(Out);
        }
    }
}
