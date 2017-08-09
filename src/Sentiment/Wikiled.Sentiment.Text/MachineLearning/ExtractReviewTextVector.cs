using System.Collections.Generic;
using System.Linq;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.NLP.NRC;
using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Text.MachineLearning
{
    public class ExtractReviewTextVector : ExtractTextVectorBase
    {
        private readonly IParsedReview review;

        public ExtractReviewTextVector(IParsedReview review)
        {
            Guard.NotNull(() => review, review);
            this.review = review;
        }

        protected override RatingData GetRating()
        {
            return review.CalculateRawRating();
        }

        protected override void Additional()
        {
            SentimentVector vector = new SentimentVector();
            vector.Extract(review.Items);
            foreach (var probability in vector.GetOccurences().Where(item => item.Probability > 0))
            {
                AddItem(null, $"DIMENSION_{probability.Data}", probability.Probability);
            }

            base.Additional();
        }

        protected override IEnumerable<ISentence> GetSentences()
        {
            return review.Sentences;
        }
    }
}
