
using Wikiled.Amazon.Logic;

namespace Wikiled.Sentiment.AcceptanceTests.Helpers.Data
{
    public class SentimentTestData
    {
        public SentimentTestData(string product, int total = 0, int errors = 0, string performance = "")
        {
            Product = product;
            Total = total;
            Errors = errors;
            Performance = performance;
        }

        public ProductCategory Category { get; set; } = ProductCategory.Electronics;

        public string Product { get; }

        public int Total { get; }

        public int Errors { get; }

        public string Performance { get; }

        public override string ToString()
        {
            return $"{Category}:{Product} Total [{Total}] Total [{Total}] Errors [{Errors}] Correct [{Performance}]";
        }
    }
}
