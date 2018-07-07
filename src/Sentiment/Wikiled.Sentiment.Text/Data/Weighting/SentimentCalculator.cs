using System;
using System.Linq;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Data.Weighting
{
    public class SentimentCalculator
    {
        private readonly SentimentValue sentimentValue;

        private readonly IWordItem wordItem;

        public SentimentCalculator(SentimentValue value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            wordItem = value.Owner;
            sentimentValue = value;
        }

        private double CalculateQuant()
        {
            var quants = wordItem.Relationship.PriorQuants.Select(item => item.QuantValue.Value).ToArray();
            return quants.Length == 0 ? 1 : quants.Average();
        }

        public SentimentValue Calculate()
        {
            var calculatedValue = sentimentValue.DataValue.Value;
            if (wordItem.Relationship.Inverted != null)
            {
                calculatedValue = -calculatedValue;
            }

            var quantitifier = CalculateQuant();
            calculatedValue = calculatedValue * quantitifier;
            var value = new SentimentValue(
                wordItem,
                new SentimentValueData(calculatedValue, sentimentValue.DataValue.SentimentSource));
            return value;
        }
    }
}
