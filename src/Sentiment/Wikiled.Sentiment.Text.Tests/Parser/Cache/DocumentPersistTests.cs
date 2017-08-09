using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sentiment.Text.Parser.Cache;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Tests.Parser.Cache
{
    [TestFixture]
    public class DocumentPersistTests
    {
        [SetUp]
        public void Setup()
        {
            if (Directory.Exists("documents"))
            {
                Directory.Delete("documents", true);
            }
        }

        [Test]
        public async Task Save()
        {
            var persist = DocumentPersist.CreateStandard(".");
            var document = new Document("Test");
            await persist.Save(document).ConfigureAwait(false);
            var documents = persist.GetDocuments("Test").ToArray();
            var allDocuments = persist.GetAllDocuments().ToArray();
            Assert.AreEqual(1, documents.Length);
            Assert.AreEqual(1, allDocuments.Length);
            Assert.AreEqual("Test", documents[0].Text);
            var document1 = await persist.GetCached("Test").ConfigureAwait(false);
            Assert.AreEqual("Test", document1.Text);
            document1 = await persist.GetCached("Root").ConfigureAwait(false);
            Assert.IsNull(document1);
        }

        [Test]
        public void SaveMultipleTimes()
        {
            var persist = DocumentPersist.CreateStandard(".");
            var document = new Document("Test");
            persist.Save(document);
            persist.Save(document);
            persist.Save(document);
            var documents = persist.GetDocuments("Test").ToArray();
            var allDocuments = persist.GetAllDocuments().ToArray();
            Assert.AreEqual(1, documents.Length);
            Assert.AreEqual(1, allDocuments.Length);
            Assert.AreEqual("Test", documents[0].Text);
        }

        [Test]
        public void SaveMultipleDocuments()
        {
            var persist = DocumentPersist.CreateStandard(".");
            var document1 = new Document("Test1");
            var document2 = new Document("Test2");
            persist.Save(document1);
            persist.Save(document2);
            var documents = persist.GetDocuments("Test1").ToArray();
            var allDocuments = persist.GetAllDocuments().ToArray();
            Assert.AreEqual(1, documents.Length);
            Assert.AreEqual(2, allDocuments.Length);
            Assert.AreEqual("Test1", documents[0].Text);
        }
    }
}
