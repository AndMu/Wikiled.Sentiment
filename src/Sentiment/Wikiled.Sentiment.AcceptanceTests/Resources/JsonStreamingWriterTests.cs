using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;
using Wikiled.Common.Extensions;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.AcceptanceTests.Resources
{
    [TestFixture]
    public class JsonStreamingWriterTests
    {
        [Test]
        public void Construct()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "out");
            path.EnsureDirectoryExistence();
            path = Path.Combine(path, "data.json");
            using (var writer = new JsonStreamingWriter(path))
            {
                writer.WriteObject(new Document("Test1"));
                writer.WriteObject(new Document("Test2"));
            }

            var result = JsonConvert.DeserializeObject<Document[]>(File.ReadAllText(path));
            Assert.AreEqual(2, result.Length);
        }
    }
}