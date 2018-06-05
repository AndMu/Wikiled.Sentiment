using System;
using NLog;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class LexiconRatingAdjustment : BaseRatingAdjustment
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private ISentimentDataReader reader;

        public LexiconRatingAdjustment(IParsedReview review, ISentimentDataReader reader) 
            : base(review)
        {
            Guard.NotNull(() => reader, reader);
            this.reader = reader;
        }

        public override void CalculateRating()
        {
            throw new NotImplementedException();
        }
    }
}
