using Wikiled.Sentiment.Analysis.Amazon;

namespace Wikiled.Sentiment.AcceptanceTests.Helpers.Data
{
    public class SentimentTestData
    {
        public SentimentTestData(string product, int total = 0, int errors = 0, string performance = "", bool cached = true)
        {
            Product = product;
            Total = total;
            Errors = errors;
            Performance = performance;
            Cached = cached;
        }

        public ProductCategory Category { get; set; } = ProductCategory.Electronics;

        public string Product { get; }

        public int Total { get; }

        public int Errors { get; }

        public string Performance { get; }

        public bool Cached { get; }

        public override string ToString()
        {
            return $"{Category}:{Product}(cached-{Cached}): Total [{Total}] Total [{Total}] Errors [{Errors}] Correct [{Performance}]";
        }
    }
}
