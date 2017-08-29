using Wikiled.Core.Utility.Arguments;

namespace Wikiled.Sentiment.Analysis.Amazon.Logic
{
    public class AmazonReview
    {
        private AmazonReview()
        {
        }

        public static AmazonReview ContructNew(ProductData product, UserData user, AmazonReviewData data, AmazonTextData textData)
        {
            var review = Construct(product, user, data, textData);
            review.Data.Id = $"{product.Id}:{user.Id}";
            review.Data.ProductId = product.Id;
            review.Data.UserId = user.Id;
            review.TextData.Id = review.Id;
            return review;
        }

        public static AmazonReview Construct(
            ProductData product, 
            UserData user, 
            AmazonReviewData data, 
            AmazonTextData textData)
        {
            Guard.NotNull(() => product, product);
            Guard.NotNull(() => user, user);
            Guard.NotNull(() => data, data);
            Guard.NotNull(() => textData, textData);
            Guard.NotNullOrEmpty(() => product.Id, product.Id);
            Guard.NotNullOrEmpty(() => user.Id, user.Id);
            AmazonReview review = new AmazonReview();
            review.Data = data;
            review.Product = product;
            review.User = user;
            review.TextData = textData;
            return review;
        }

        public string Id => Data.Id;

        public AmazonTextData TextData { get; private set; }

        public AmazonReviewData Data { get; private set; }

        public ProductData Product { get; private set; }

        public UserData User { get; private set; }
    }
}
