using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System.Linq;
using Wikiled.Sentiment.Analysis.Processing.Persistency;

namespace Wikiled.Sentiment.Integration.Tests.Processing.Persistency
{
    [TestFixture]
    public class CsvDataSourceTests
    {
        [Test]
        public void GetReviews()
        {
            var source = new CsvDataSource(new NullLogger<CsvDataSource>(), Path.Combine(TestContext.CurrentContext.TestDirectory, @"Processing\Persistency\data.csv"));
            var reviews = source.GetReviews().ToArray();
            Assert.AreEqual(4, reviews.Length);
            Assert.AreEqual("561697961110167552", reviews[0].Data.Result.Id);
            Assert.AreEqual(1, reviews[0].Data.Result.Stars);
            Assert.AreEqual("putting apple's record-breaking quarter into context: $aapl URL_URL via @forbestech", reviews[0].Data.Result.Text);
        }
    }
}