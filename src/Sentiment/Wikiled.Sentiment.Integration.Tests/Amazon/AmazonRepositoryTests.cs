using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Redis.Config;
using Wikiled.Redis.Logic;
using Wikiled.Sentiment.Analysis.Amazon;
using Wikiled.Sentiment.Analysis.Amazon.Logic;

namespace Wikiled.Sentiment.Integration.Tests.Amazon
{
    [TestFixture]
    public class AmazonRepositoryTests
    {
        private IRedisLink link;

        private AmazonRepository instance;

        private AmazonReview review;

        [OneTimeSetUp]
        public void Setup()
        {
            link = new RedisLink("Test", new RedisMultiplexer(new RedisConfiguration("localhost", 6666)));
            link.Open();
            instance = new AmazonRepository(link);
            review = AmazonReview.Construct(
                new ProductData { Id = "Product1" },
                new UserData { Id = "User1" },
                new AmazonReviewData { Id = "One", UserId = "User1", ProductId = "Product1" },
                new AmazonTextData { Text = "Test" });
            review.User.Name = "Andrius";
            review.Product.Name = "Nokia";
            review.Product.Category = ProductCategory.Electronics;
            review.Data.Date = new DateTime(2012, 01, 01);
            review.Product.Price = 10;
            var task1 = instance.Save(review);
            review.Data.Id = "Two";
            review = AmazonReview.Construct(review.Product, review.User, review.Data, review.TextData);
            var task2 = instance.Save(review);
            Task.WaitAll(task1, task2);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            link.Close();
        }

        [Test]
        public async Task LoadUser()
        {
            var user = await instance.LoadUser("User1").ConfigureAwait(false);
            Assert.AreEqual("User1", user.Id);
            Assert.AreEqual("Andrius", user.Name);
            user = await instance.LoadUser("xxx").ConfigureAwait(false);
            Assert.IsNull(user);
        }

        [Test]
        public async Task LoadProduct()
        {
            var product = await instance.LoadProduct("Product1").ConfigureAwait(false);
            Assert.AreEqual("Product1", product.Id);
            Assert.AreEqual("Nokia", product.Name);
            Assert.AreEqual(10, product.Price);
            Assert.AreEqual(ProductCategory.Electronics, product.Category);
            product = await instance.LoadProduct("xxx").ConfigureAwait(false);
            Assert.IsNull(product);
        }

        [Test]
        public async Task Load()
        {
            var amazon = await instance.Load("One");
            Assert.AreEqual("Product1", amazon.Product.Id);
            Assert.AreEqual("Nokia", amazon.Product.Name);
            Assert.AreEqual(10, amazon.Product.Price);
            Assert.AreEqual(ProductCategory.Electronics, amazon.Product.Category);
            Assert.AreEqual("User1", amazon.User.Id);
            Assert.AreEqual("Andrius", amazon.User.Name);
            amazon = await instance.Load("xxx").ConfigureAwait(false);
            Assert.IsNull(amazon);
        }

        [Test]
        public void LoadAll()
        {
            var reviews = instance.LoadAll(ProductCategory.Electronics).ToEnumerable().ToArray();
            Assert.AreEqual(2, reviews.Length);
            reviews = instance.LoadAll(ProductCategory.Medic).ToEnumerable().ToArray(); 
            Assert.AreEqual(0, reviews.Length);
        }

        [Test]
        public void LoadAllYear()
        {
            var reviews = instance.LoadAll(2012, ProductCategory.Electronics).ToEnumerable().ToArray(); 
            Assert.AreEqual(2, reviews.Length);
            reviews = instance.LoadAll(2014, ProductCategory.Electronics).ToEnumerable().ToArray();
            Assert.AreEqual(0, reviews.Length);
        }

        [Test]
        public void FindReviews()
        {
            var amazon = instance.LoadProductReviews("Product1").ToEnumerable().ToArray();
            Assert.AreEqual(2, amazon.Length);
        }
    }
}
