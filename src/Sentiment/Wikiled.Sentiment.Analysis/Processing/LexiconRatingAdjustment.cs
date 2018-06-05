using System;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class LexiconRatingAdjustment : BaseRatingAdjustment
    {
        private ISentimentDataReader reader;

        public LexiconRatingAdjustment(IParsedReview review, ISentimentDataReader reader) 
            : base(review)
        {
            Guard.NotNull(() => reader, reader);
            this.reader = reader;
            throw new NotImplementedException();
        }
    }
}
