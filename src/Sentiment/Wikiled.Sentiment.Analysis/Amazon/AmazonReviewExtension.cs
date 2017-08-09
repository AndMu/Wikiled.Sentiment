using System;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Amazon
{
    public static class AmazonReviewExtension
    {
        public static SingleProcessingData CreateProcessingData(this AmazonReview amazonReview)
        {
            var document = new Document(amazonReview.TextData.Text);
            document.Stars = amazonReview.Data.Score;
            document.Id = amazonReview.Id;
            document.DocumentTime = UnixTimeStampToDateTime(amazonReview.Data.Time);
            document.Author = amazonReview.User.Id;
            return new SingleProcessingData(document) { Stars = amazonReview.Data.Score };
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

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
