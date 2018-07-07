using System;
using NLog;
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
            if (documents is null)
            {
                throw new ArgumentNullException(nameof(documents));
            }

            if (documents.Length == 0)
            {
                throw new ArgumentException("Value cannot be an empty collection.", nameof(documents));
            }

            this.documents = documents;
        }

        public IStatisticsResult GetStatistics(int windowSize)
        {
            if (windowSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(windowSize));
            }

            log.Debug("GetStatistics: <{0}>", windowSize);
            result = new StatisticsResult(GetValue(), windowSize);
            documents.ForEachExecute(document => Process(document, result));
            return result;
        }

        protected abstract double? GetValue();

        protected abstract void Process(Document currentDocument, StatisticsResult result);
    }
}
