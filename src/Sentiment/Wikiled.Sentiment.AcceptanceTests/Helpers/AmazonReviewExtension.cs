using Wikiled.Amazon.Logic;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.AcceptanceTests.Helpers
{
    public static class AmazonReviewExtension
    {
        public static Document CreateDocument(this AmazonReview review)
        {
            return new Document(review.TextData.Text)
            {
                Id = review.Id,
                Stars = review.Data.Score
            };
        }
    }
}
