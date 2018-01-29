using System;
using System.IO;
using com.sun.xml.@internal.bind.v2;
using Newtonsoft.Json;
using NUnit.Framework;
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
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "out", "data.json");
            using (var writer = new JsonStreamingWriter<Document>(path))
            {
                writer.WriteObject(new Document("Test1"));
                writer.WriteObject(new Document("Test2"));
            }

            var result = JsonConvert.DeserializeObject<Document[]>(File.ReadAllText(path));
            Assert.AreEqual(2, result.Length);
        }
    }
}