using System;
using System.Collections.Generic;
using Wikiled.Arff.Persistence;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class ProcessingData
    {
        private List<SingleProcessingData> negative = new List<SingleProcessingData>();

        private List<SingleProcessingData> neutral = new List<SingleProcessingData>();

        private List<SingleProcessingData> positive = new List<SingleProcessingData>();

        public IEnumerable<SingleProcessingData> Negative
        {
            get => negative.ToArray();
            set => negative = new List<SingleProcessingData>(value);
        }

        public IEnumerable<SingleProcessingData> Neutral
        {
            get => neutral.ToArray();
            set => neutral = new List<SingleProcessingData>(value);
        }

        public IEnumerable<SingleProcessingData> Positive
        {
            get => positive;
            set => positive = new List<SingleProcessingData>(value);
        }

        public void Add(PositivityType positivity, SingleProcessingData data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            switch (positivity)
            {
                case PositivityType.Positive:
                    positive.Add(data);
                    break;
                case PositivityType.Negative:
                    negative.Add(data);
                    break;
                case PositivityType.Neutral:
                    neutral.Add(data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(positivity));
            }
        }

        public override string ToString()
        {
            return $"Articles Positive:{positive.Count} Negative:{negative.Count} Neutral:{neutral.Count}";
        }
    }
}
