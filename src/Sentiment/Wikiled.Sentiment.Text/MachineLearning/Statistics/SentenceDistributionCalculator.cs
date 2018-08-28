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
            return Sentence.CalculateSentiment().RawRating;
        }

        protected override void Process(Document currentDocument, StatisticsResult result)
        {
            var autoEvictingDictionary = new AutoEvictingDictionary<SentenceItem, IStatisticsResult>(length: result.WindowsSize);
            for (int i = 0; i < currentDocument.Sentences.Count; i++)
            {
                autoEvictingDictionary.Increment();
                SentenceItem currentSentence = currentDocument.Sentences[i];
                IStatisticsResult statistics = currentSentence == Sentence
                                                   ? (IStatisticsResult)result
                                                   : new NullStatisticsResult(currentSentence.CalculateSentiment().RawRating);

                foreach (var statisticsResult in autoEvictingDictionary.Values)
                {
                    statisticsResult.AddData(currentSentence.CalculateSentiment().RawRating);
                    statistics.AddData(statisticsResult.Value);
                }

                autoEvictingDictionary.Add(currentSentence, statistics);
                result.IncrementTotal();
                statistics.IncrementOccurences();
            }
        }
    }
}
