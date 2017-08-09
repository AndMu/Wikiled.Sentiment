using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Wikiled.Arff.Persistence;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class ProcessingData : IProcessingData
    {
        private List<SingleProcessingData> negative = new List<SingleProcessingData>();

        private List<SingleProcessingData> neutral = new List<SingleProcessingData>();

        private List<SingleProcessingData> positive = new List<SingleProcessingData>();

        [XmlIgnore]
        public IEnumerable<SingleProcessingData> AllReviews => GetReviews(negative, positive, neutral);

        [XmlArray]
        [XmlArrayItem("DataItem")]
        public SingleProcessingData[] Negative
        {
            get => negative.ToArray();
            set => negative = new List<SingleProcessingData>(value);
        }

        [XmlArray]
        [XmlArrayItem("DataItem")]
        public SingleProcessingData[] Neutral
        {
            get => neutral.ToArray();
            set => neutral = new List<SingleProcessingData>(value);
        }

        [XmlArray]
        [XmlArrayItem("DataItem")]
        public SingleProcessingData[] Positive
        {
            get => positive.ToArray();
            set => positive = new List<SingleProcessingData>(value);
        }

        public void Add(PositivityType positivity, SingleProcessingData data)
        {
            Guard.NotNull(() => data, data);
            switch(positivity)
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

        private IEnumerable<SingleProcessingData> GetReviews(
            IEnumerable<SingleProcessingData> negativeItems,
            IEnumerable<SingleProcessingData> positiveItems,
            IEnumerable<SingleProcessingData> neutralItems)
        {
            var reviews = new List<SingleProcessingData>();
            reviews.AddRange(negativeItems);
            reviews.AddRange(positiveItems);
            reviews.AddRange(neutralItems);
            return reviews.OrderBy(item => item.Date);
        }
    }
}
