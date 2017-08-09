using System.IO;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Wikiled.Arff.Persistence;
using Wikiled.Core.Utility.Serialization;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Workspace.Data;
using Wikiled.Sentiment.Analysis.Workspace.Data.Selection;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Tests.Workspace.Data
{
    [TestFixture]
    public class DataSetTests
    {
        private MockRepository mock;
        private DataConfiguration dataConfiguration;

        [SetUp]
        public void Setup()
        {
            mock = new MockRepository();
            dataConfiguration = new DataConfiguration();
            dataConfiguration.Configurations.Add(new ItemConfiguration());
            dataConfiguration.Configurations.Add(new ItemConfiguration());
            dataConfiguration.Configurations[0].Stars = 5;
        }

        [Test]
        public void IsReadyTesting()
        {
            mock.ReplayAll();
            var data = new DataSet(true, dataConfiguration);
            Assert.IsTrue(data.IsTesting);
            Assert.AreEqual(2, data.Collections.Count());
            Assert.IsFalse(data.IsReady);
            data.Collections.First().Items.Add(new SingleProcessingData());
            Assert.IsTrue(data.IsReady);
            mock.VerifyAll();
        }

        [Test]
        public void AddPSentiFile()
        {
            var data = new ProcessingData();
            data.Positive = new[] { new SingleProcessingData(), new SingleProcessingData() };
            data.Negative = new[] { new SingleProcessingData() };
            data.XmlSerialize().Save("data.xml");
            DataSet dataSet = new DataSet(false, dataConfiguration);
            dataSet.AddPFile(new PSentiFileSelection("data.xml"));
            Assert.AreEqual(2, dataSet.Collections.FirstOrDefault(item => item.PositivityType == PositivityType.Positive).Items.Count);
            Assert.AreEqual(1, dataSet.Collections.FirstOrDefault(item => item.PositivityType == PositivityType.Negative).Items.Count);
            Assert.AreEqual(1, dataConfiguration.PSentiSources.Count);
        }


        [Test]
        public void IsReadyTraining()
        {
            mock.ReplayAll();
            var data = new DataSet(false, dataConfiguration);
            Assert.IsFalse(data.IsTesting);
            Assert.AreEqual(2, data.Collections.Count());
            for (int i = 0; i < 10; i++)
            {
                Assert.IsFalse(data.IsReady);
                data.Collections.First().Items.Add(new SingleProcessingData());
                data.Collections.Skip(1).First().Items.Add(new SingleProcessingData());
            }
            Assert.IsTrue(data.IsReady);
            mock.VerifyAll();
        }

        [Test]
        public void Changed()
        {
            mock.ReplayAll();
            var data = new DataSet(false, dataConfiguration);
            int total = 0;
            data.Changed += (sender, args) => total++;
            Assert.AreEqual(0, total);
            data.Collections.First().AddFiles(Path.Combine(TestContext.CurrentContext.TestDirectory, @"Workspace\Data\TestData\1.txt"));
            Assert.GreaterOrEqual(total, 1);
            mock.VerifyAll();
        }

        [Test]
        public void Data()
        {
            var data = new DataSet(false, dataConfiguration);
            data.Collections.First().Items.Add(new SingleProcessingData { Stars = 1 });
            var dataCollection = data.Data.ToArray();
            Assert.AreEqual(1, dataCollection.Length);
            Assert.AreEqual(1, dataCollection[0].Stars);
        }

        [Test]
        public void LoadDocuments()
        {
            var documents = new Document[1];
            documents[0] = new Document("Second");
            documents.XmlSerialize().Save("results.xml");
            var data = new DataSet(false, dataConfiguration);
            var simple1 = new SingleProcessingData();
            simple1.Text = "First ";
            var simple2 = new SingleProcessingData();
            simple2.Text = "second ";
            data.Collections.First().Items.Add(simple1);
            data.Collections.First().Items.Add(simple2);
            Assert.IsNull(simple1.Document);
            Assert.IsNull(simple2.Document);
            data.LoadDocuments(".");
            Assert.IsNull(simple1.Document);
            Assert.IsNotNull(simple2.Document);
        }
    }
}
