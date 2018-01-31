using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Async;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.MachineLearning.Statistics
{
    public abstract class BaseDistributionCalculator
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly Document[] documents;

        private StatisticsResult result;

        protected BaseDistributionCalculator(params Document[] documents)
        {
            Guard.NotEmpty(() => documents, documents);
            this.documents = documents;
        }

        public IStatisticsResult GetStatistics(int windowSize)
        {
            Guard.IsValid(() => windowSize, windowSize, item => item > 0, "windowSize");
            log.Debug("GetStatistics: <{0}>", windowSize);
            result = new StatisticsResult(GetValue(), windowSize);
            documents.ForEachExecute(document => Process(document, result));
            return result;
        }

        protected abstract double? GetValue();

        protected abstract void Process(Document currentDocument, StatisticsResult result);
    }
}
