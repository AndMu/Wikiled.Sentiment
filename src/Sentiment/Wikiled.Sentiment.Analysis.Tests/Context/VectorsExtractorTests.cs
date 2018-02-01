using System.IO;
using System.Linq;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Processing.Context;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Tests.Context
{
    [TestFixture]
    public class VectorsExtractorTests
    {
        private Document document;

        private DirectoryInfo info;

        [SetUp]
        public void Setup()
        {
            document = new Document("Test");
            for (int i = 0; i < 10; i++)
            {
                SentenceItem sentence = new SentenceItem("S1");
                sentence.Add("w1");
                sentence.Add("w2");
                sentence.Add("w3");
                document.Add(sentence);
                foreach (var wordEx in sentence.Words)
                {
                    wordEx.Value = 1;
                }
            }

            info = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "."));
            foreach (var file in info.GetFiles("*.arff"))
            {
                file.Delete();
            }
        }

        [Test]
        public void Constructor()
        {
            VectorsExtractor vectors = new VectorsExtractor(10);
            Assert.AreEqual(10, vectors.WindowSize);
            Assert.AreEqual(0, vectors.WordVectors.Count);
        }

        [Test]
        public void Process()
        {
            VectorsExtractor vectors = new VectorsExtractor(3);
            vectors.Process(document);
            Assert.AreEqual(3, vectors.WordVectors.Count);
            for (int i = 0; i < 3; i++)
            {
                var vectorsData = vectors.WordVectors.ToArray()[i];
                for (int j = 0; j < vectorsData.Vectors.Count; j++)
                {
                    int total = 10;
                    if (j == 0 ||
                        j == 9)
                    {
                        total = 6;
                    }
                    else if (j == 1 ||
                        j == 8)
                    {
                        total = 8;
                    }

                    Assert.AreEqual(total, vectorsData.Vectors[j].Words.Count);
                }
            }
        }

        [Test]
        public void Save()
        {
            VectorsExtractor vectors = new VectorsExtractor(3);
            vectors.Process(document);
            vectors.Save(info.FullName);
            var files = info.GetFiles("*.arff");
            Assert.AreEqual(3, files.Length);
        }
    }
}
