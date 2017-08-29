using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Redis.Indexing;
using Wikiled.Redis.Keys;
using Wikiled.Redis.Logic;
using Wikiled.Redis.Persistency;

namespace Wikiled.Sentiment.Analysis.Amazon.Logic
{
    public class AmazonRepository : IRepository
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IRedisLink manager;

        private readonly ConcurrentDictionary<string, ProductData> products = new ConcurrentDictionary<string, ProductData>();

        private readonly Dictionary<string, AmazonReview> reviews = new Dictionary<string, AmazonReview>();

        private readonly ConcurrentDictionary<string, UserData> users = new ConcurrentDictionary<string, UserData>();

        public AmazonRepository(IRedisLink manager)
        {
            Guard.NotNull(() => manager, manager);
            this.manager = manager;
            manager.RegisterHashType<AmazonReviewData>().IsSingleInstance = true;
            manager.RegisterHashType<UserData>().IsSingleInstance = true;
            manager.RegisterHashType<ProductData>().IsSingleInstance = true;
            manager.RegisterWellknown<AmazonTextData>().IsSingleInstance = true;
        }

        public string Name => "Amazon";

        public IObservable<AmazonReview> LoadProductReviews(string productId)
        {
            Guard.NotNullOrEmpty(() => productId, productId);
            logger.Debug("FindReviews :<{0}>", productId);
            return LoadReviewsByIndex(GetProductIndexKey(productId));
        }
        
        public IObservable<AmazonReview> LoadReviewsByIndex(string key)
        {
            var current = manager.Client.GetRecords<AmazonReviewData>(new IndexKey(this, key, false));
            return current.Select(GetAmazonReview).Merge();
        }

        public IObservable<UserData> FindUsers(string productId)
        {
            Guard.NotNullOrEmpty(() => productId, productId);
            logger.Debug("FindUsers :<{0}>", productId);
            return manager.Client.GetRecords<UserData>(GetProductUserIndex(productId));
        }

        public async Task<AmazonReview> Load(string id)
        {
            Guard.NotNullOrEmpty(() => id, id);
            logger.Debug("Load:<{0}>", id);
            var key = GetAmazonKey(id);
            var result = await manager.Client.GetRecords<AmazonReviewData>(key).FirstOrDefaultAsync();
            if (result?.Id != null)
            {
                return await GetAmazonReview(result).ConfigureAwait(false);
            }

            return null;
        }

        public IObservable<AmazonReview> LoadAll(ProductCategory category)
        {
            var all = GetProductTypeIndex(category);
            return LoadReviewsByIndex(all);
        }

        public IObservable<AmazonReview> LoadAll(int year, ProductCategory category)
        {
            var all = GetProductTypeYearIndex(year, category);
            return LoadReviewsByIndex(all);
        }

        public async Task<ProductData> LoadProduct(string id)
        {
            Guard.NotNullOrEmpty(() => id, id);
            logger.Debug("LoadProduct:<{0}>", id);
            ProductData productData;
            if (!products.TryGetValue(id, out productData))
            {
                var key = GetProductKey(id);
                productData = await manager.Client.GetRecords<ProductData>(key).FirstOrDefaultAsync();
                if (productData != null &&
                    productData.Id == id)
                {
                    products[id] = productData;
                }
                else
                {
                    return null;
                }
            }

            return productData;
        }

        public async Task<UserData> LoadUser(string id)
        {
            Guard.NotNullOrEmpty(() => id, id);
            logger.Debug("LoadUser:<{0}>", id);
            UserData user;
            if (!users.TryGetValue(id, out user))
            {
                var key = GetUserKey(id);
                user = await manager.Client.GetRecords<UserData>(key).FirstOrDefaultAsync();
                if (user != null &&
                    user.Id == id)
                {
                    users[id] = user;
                }
                else
                {
                    return null;
                }
            }

            return user;
        }

        public async Task Save(AmazonReview review)
        {
            Guard.NotNull(() => review, review);
            logger.Debug("Save:<{0}>", review.Data.Id);
            lock (reviews)
            {
                if (reviews.ContainsKey(review.Id))
                {
                    return;
                }

                reviews[review.Id] = review;
            }

            var key = GetFullAmazonKey(review);
            var contains = await manager.Client.ContainsRecord<AmazonReviewData>(key).ConfigureAwait(false);
            if (contains)
            {
                logger.Debug("Record already exist: {0}", review.Data.Id);
                return;
            }

            var task = Save(review.Product, review.User);
            var textData = manager.Client.AddRecord(GetAmazonDataKey(review.Id), review.TextData);
            var secondTask = manager.Client.AddRecord(key, review.Data);
            await Task.WhenAll(task, secondTask, textData).ConfigureAwait(false);
        }

        private IDataKey GetFullAmazonKey(AmazonReview review)
        {
            var key = GetAmazonKey(review.Data.Id);
            key.AddIndex(new IndexKey(this, GetProductIndexKey(review.Product.Id), false));
            key.AddIndex(new IndexKey(this, GetUserIndexKey(review.User.Id), false));
            key.AddIndex(new IndexKey(this, GetProductTypeIndex(review.Product.Category), false));
            key.AddIndex(new IndexKey(this, GetProductTypeYearIndex(review.Data.Date.Year, review.Product.Category), false));
            return key;
        }

        private string GetUserIndexKey(string id)
        {
            return $"Index:Users:Reviews:{id}";
        }

        private string GetProductIndexKey(string id)
        {
            return $"Index:Products:Reviews:{id}";
        }

        public Task<long> CountReviews(ProductCategory category)
        {
            var index = GetProductTypeIndex(category);
            return new IndexManagerFactory(manager, manager.Database).Create(new IndexKey(this, index, false)).Count();
        }

        private Task Save(ProductData productData, UserData user)
        {
            Guard.NotNull(() => productData, productData);
            Guard.NotNull(() => user, user);
            var productKey = GetProductKey(productData.Id);
            var userKey = GetUserKey(user.Id);
            productKey.AddIndex(GetUserProductsIndex(user.Id));
            userKey.AddIndex(GetProductUserIndex(productData.Id));

            var productTask = Save(productKey, productData);
            var userTask = Save(userKey, user);
            return Task.WhenAll(productTask, userTask);
        }

        private async Task Save(IDataKey key, ProductData productData)
        {
            lock (products)
            {
                if (products.ContainsKey(productData.Id))
                {
                    return;
                }

                products[productData.Id] = productData;
            }

            var contains = await manager.Client.ContainsRecord<ProductData>(key).ConfigureAwait(false);
            if (!contains)
            {
                await manager.Client.AddRecord(key, productData).ConfigureAwait(false);
            }
        }

        private async Task Save(IDataKey key, UserData user)
        {
            lock (users)
            {
                if (users.ContainsKey(user.Id))
                {
                    return;
                }

                users[user.Id] = user;
            }

            var contains = await manager.Client.ContainsRecord<UserData>(key).ConfigureAwait(false);
            if (!contains)
            {
                await manager.Client.AddRecord(key, user).ConfigureAwait(false);
            }
        }

        private RepositoryKey GetAmazonKey(string id)
        {
            RepositoryKey userKey = new RepositoryKey(this, new ObjectKey(id));
            return userKey;
        }

        private RepositoryKey GetAmazonDataKey(string id)
        {
            RepositoryKey userKey = new RepositoryKey(this, new ObjectKey("Text", id));
            return userKey;
        }

        private async Task<AmazonReview> GetAmazonReview(AmazonReviewData data)
        {
            var userTask = LoadUser(data.UserId);
            var productTask = LoadProduct(data.ProductId);
            var textData = await manager.Client.GetRecords<AmazonTextData>(GetAmazonDataKey(data.Id)).FirstAsync();
            await Task.WhenAll(userTask, productTask).ConfigureAwait(false);
            var review = AmazonReview.Construct(productTask.Result, userTask.Result, data, textData);
            return review;
        }

        private RepositoryKey GetProductKey(string id)
        {
            RepositoryKey productKey = new RepositoryKey(this, new ObjectKey("Product", id));
            productKey.AddIndex(new IndexKey(this, "Index:Products:All", false));
            return productKey;
        }

        private string GetProductTypeIndex(ProductCategory category)
        {
            return $"Index:{category}:All";
        }

        private string GetProductTypeYearIndex(int year, ProductCategory category)
        {
            return $"Index:Year:{category}:{year}";
        }

        private IndexKey GetProductUserIndex(string productId)
        {
            return new IndexKey(this, $"Index:Products:Users:{productId}", false);
        }

        private RepositoryKey GetUserKey(string id)
        {
            RepositoryKey userKey = new RepositoryKey(this, new ObjectKey("User", id));
            userKey.AddIndex(new IndexKey(this, "Index:Users:All", false));
            return userKey;
        }

        private IndexKey GetUserProductsIndex(string userId)
        {
            return new IndexKey(this, $"Index:Users:Products:{userId}", false);
        }
    }
}
