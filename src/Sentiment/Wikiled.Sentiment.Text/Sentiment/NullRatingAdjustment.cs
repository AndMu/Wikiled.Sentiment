using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public class NullRatingAdjustment : IRatingAdjustment
    {
        public static NullRatingAdjustment Instance { get; } = new NullRatingAdjustment();

        private NullRatingAdjustment()
        {
        }

        public RatingData Rating => null;

        public SentimentValue GetSentiment(IWordItem word)
        {
            return null;
        }
    }
}
