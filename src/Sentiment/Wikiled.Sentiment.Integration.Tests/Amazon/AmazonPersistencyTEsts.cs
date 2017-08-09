using System;
using System.Linq;
using NUnit.Framework;
using Wikiled.Redis.Config;
using Wikiled.Redis.Logic;
using Wikiled.Sentiment.Analysis.Amazon;
using Wikiled.Sentiment.Text.Parser.Cache;

namespace Wikiled.Sentiment.Integration.Tests.Amazon
{
    [TestFixture]
    public class AmazonPersistencyTests
    {
        private IRedisLink link;

        private AmazonRepository instance;

        private AmazonReview review;

        [OneTimeSetUp]
        public void Setup()
        {
            link = new RedisLink("Test", new RedisMultiplexer(new RedisConfiguration("localhost", 6666)));
            link.Open();
            instance = new AmazonRepository(link, NullCachedDocumentsSource.Instance);
            review = new AmazonReview(new ProductData { Id = "Product1" }, new UserData { Id = "User1" }, new AmazonReviewData { Id = "One" });
            review.Data.Text = "Test";
            review.User.Name = "Andrius";
            review.Product.Name = "Nokia";
            review.Product.Category = ProductCategory.Electronics;
            review.Data.Date = new DateTime(2012, 01, 01);
            review.Product.Price = 10;
            instance.Save(review);
            review.Data.Id = "Two";
            review = new AmazonReview(review.Product, review.User, review.Data);
            instance.Save(review);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            link.Close();
        }

        [Test]
        public void LoadUser()
        {
            var user = instance.LoadUser("User1");
            Assert.AreEqual("User1", user.Id);
            Assert.AreEqual("Andrius", user.Name);
            user = instance.LoadUser("xxx");
            Assert.IsNull(user);
        }

        [Test]
        public void LoadProduct()
        {
            var product = instance.LoadProduct("Product1");
            Assert.AreEqual("Product1", product.Id);
            Assert.AreEqual("Nokia", product.Name);
            Assert.AreEqual(10, product.Price);
            Assert.AreEqual(ProductCategory.Electronics, product.Category);
            product = instance.LoadProduct("xxx");
            Assert.IsNull(product);
        }

        [Test]
        public void Load()
        {
            var amazon = instance.Load("One");
            Assert.AreEqual("Product1", amazon.Product.Id);
            Assert.AreEqual("Nokia", amazon.Product.Name);
            Assert.AreEqual(10, amazon.Product.Price);
            Assert.AreEqual(ProductCategory.Electronics, amazon.Product.Category);
            Assert.AreEqual("User1", amazon.User.Id);
            Assert.AreEqual("Andrius", amazon.User.Name);
            amazon = instance.Load("xxx");
            Assert.IsNull(amazon);
        }

        [Test]
        public void LoadAll()
        {
            var reviews = instance.LoadAll(ProductCategory.Electronics).ToArray();
            Assert.AreEqual(2, reviews.Length);
            reviews = instance.LoadAll(ProductCategory.Medic).ToArray();
            Assert.AreEqual(0, reviews.Length);
        }

        [Test]
        public void LoadAllYear()
        {
            var reviews = instance.LoadAll(2012, ProductCategory.Electronics).ToArray();
            Assert.AreEqual(2, reviews.Length);
            reviews = instance.LoadAll(2014, ProductCategory.Electronics).ToArray();
            Assert.AreEqual(0, reviews.Length);
        }

        [Test]
        public void FindReview()
        {
            var amazon = instance.FindReview("User1", "Product1");
            Assert.AreEqual("Product1", amazon.Product.Id);
            Assert.AreEqual("Nokia", amazon.Product.Name);
            Assert.AreEqual(10, amazon.Product.Price);
            Assert.AreEqual(ProductCategory.Electronics, amazon.Product.Category);
            Assert.AreEqual("User1", amazon.User.Id);
            Assert.AreEqual("Andrius", amazon.User.Name);
            amazon = instance.FindReview("User1", "xxx");
            Assert.IsNull(amazon);
        }

        [Test]
        public void FindReviews()
        {
            var amazon = instance.FindReviews("Product1").ToArray();
            Assert.AreEqual(2, amazon.Length);
        }
    }
}
