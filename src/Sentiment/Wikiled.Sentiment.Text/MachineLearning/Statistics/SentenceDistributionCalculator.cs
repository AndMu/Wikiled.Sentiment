using System;
using Wikiled.Sentiment.Text.Helpers;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.MachineLearning.Statistics
{
    public class SentenceDistributionCalculator : BaseDistributionCalculator
    {
        public SentenceDistributionCalculator(SentenceItem sentence, Document document)
            : base(document)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            Sentence = sentence ?? throw new ArgumentNullException(nameof(sentence));
        }

        public SentenceItem Sentence { get; }

        protected override double? GetValue()
        {
            return Sentence.CalculateSentiment();
        }

        protected override void Process(Document currentDocument, StatisticsResult result)
        {
            var autoEvictingDictionary = new AutoEvictingDictionary<SentenceItem, IStatisticsResult>(length: result.WindowsSize);
            for (int i = 0; i < currentDocument.Sentences.Count; i++)
            {
                autoEvictingDictionary.Increment();
                SentenceItem currenSentence = currentDocument.Sentences[i];
                IStatisticsResult statistics = currenSentence == Sentence
                                                   ? (IStatisticsResult)result
                                                   : new NullStatisticsResult(currenSentence.CalculateSentiment());

                foreach (var statisticsResult in autoEvictingDictionary.Values)
                {
                    statisticsResult.AddData(currenSentence.CalculateSentiment());
                    statistics.AddData(statisticsResult.Value);
                }

                autoEvictingDictionary.Add(currenSentence, statistics);
                result.IncrementTotal();
                statistics.IncrementOccurences();
            }
        }
    }
}
