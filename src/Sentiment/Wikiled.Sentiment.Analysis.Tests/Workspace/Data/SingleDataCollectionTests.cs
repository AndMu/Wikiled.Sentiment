using System.IO;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Workspace.Data;

namespace Wikiled.Sentiment.Analysis.Tests.Workspace.Data
{
    [TestFixture]
    public class SingleDataCollectionTests
    {
        [Test]
        public void Construct()
        {
            var configuration = new ItemConfiguration();
            configuration.Name = "Test";
            configuration.Stars = 3;
            SingleDataCollection collection = new SingleDataCollection(configuration);
            Assert.AreEqual(configuration.Name, collection.Name);
            Assert.AreEqual(configuration.Stars, collection.Stars);
            Assert.AreEqual(configuration, collection.Configuration);
            Assert.AreEqual(0, collection.Items.Count);
        }

        [Test]
        public void ConstructWithInit()
        {
            var configuration = new ItemConfiguration();
            configuration.Name = "Test";
            configuration.Stars = 3;
            configuration.Folders.Add(Path.Combine(TestContext.CurrentContext.TestDirectory, @"Workspace\Data\TestData"));
            SingleDataCollection collection = new SingleDataCollection(configuration);
            int totalChanged = 0;
            collection.Changed += (sender, args) => totalChanged++;
            Assert.AreEqual(configuration.Name, collection.Name);
            Assert.AreEqual(configuration.Stars, collection.Stars);
            Assert.AreEqual(configuration, collection.Configuration);
            Assert.IsTrue(collection.Items.Count >= 3);
            Assert.AreEqual(3, collection.Items[0].Stars);
            Assert.AreEqual(0, totalChanged);
        }

        [Test]
        public void AddFile()
        {
            var configuration = new ItemConfiguration();
            SingleDataCollection collection = new SingleDataCollection(configuration);
            int totalChanged = 0;
            collection.Changed += (sender, args) => totalChanged++;
            Assert.AreEqual(0, collection.Items.Count);
            collection.AddFiles(Path.Combine(TestContext.CurrentContext.TestDirectory, @"Workspace\Data\TestData\3.txt"));
            Assert.AreEqual(1, collection.Items.Count);
            Assert.AreEqual(1, collection.Configuration.Files.Count);
            Assert.AreEqual(2, totalChanged);
        }

        [Test]
        public void AddFileTwice()
        {
            var configuration = new ItemConfiguration();
            SingleDataCollection collection = new SingleDataCollection(configuration);
            Assert.AreEqual(0, collection.Items.Count);
            collection.AddFiles(Path.Combine(TestContext.CurrentContext.TestDirectory, @"Workspace\Data\TestData\3.txt"));
            Assert.AreEqual(1, collection.Items.Count);
            Assert.AreEqual(1, collection.Configuration.Files.Count);
            collection.AddFiles(Path.Combine(TestContext.CurrentContext.TestDirectory, @"Workspace\Data\TestData\3.tXt"));
            Assert.AreEqual(1, collection.Items.Count);
            Assert.AreEqual(1, collection.Configuration.Files.Count);
        }

        [Test]
        public void AddFolder()
        {
            var configuration = new ItemConfiguration();
            SingleDataCollection collection = new SingleDataCollection(configuration);
            Assert.AreEqual(0, collection.Items.Count);
            collection.AddFolder(Path.Combine(TestContext.CurrentContext.TestDirectory, @"Workspace\Data\TestData"));
            Assert.IsTrue(collection.Items.Count >= 3);
            Assert.AreEqual(1, collection.Configuration.Folders.Count);
        }
    }
}
